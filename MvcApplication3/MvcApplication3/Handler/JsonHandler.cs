using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using System.Web.WebPages;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using MvcApplication3.Common;
using MvcApplication3.Common.Extension;
using MvcApplication3.Models;
using MvcApplication3.Service;
using MvcApplication3.ServiceFactory;
using MvcApplication3.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;


namespace MvcApplication3.Handler
{
    public class JsonHandler : MvcHandler, IRequiresSessionState
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 请求开始时间
        /// </summary>
        private DateTime start_Time;

        /// <summary>
        /// 请求结束时间
        /// </summary>
        private DateTime end_Time;

        /// <summary>
        /// 请求参数
        /// </summary>
        private String requestData;

        public JsonHandler(RequestContext context)
            : base(context)
        {
            ProcessRequest(context);
        }

        private string ReadContext(HttpRequestBase request)
        {
            try
            {
                using (var stream = request.InputStream)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        string body = sr.ReadToEnd();
                        return body;
                    }
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        //得到service 的实例来执行action 方法
        private IService CreateServiceInstance(RequestContext requestContext)
        {
            var serviceFactory = new ReflelctionServiceFactory();
            string serviceName = requestContext.RouteData.GetRequiredString("controller");
            return serviceFactory.CreateService(requestContext, serviceName);
        }
        //get log message
        private StringBuilder SetLogMessage(RequestContext requestContext, int status, string responseData, string des)
        {
            var message = new StringBuilder();
            message.Append("URL=");
            message.Append(requestContext.HttpContext.Request.Url + "|");
            message.Append("Status=");
            message.Append(status + "|");
            message.Append("IP=");
            message.Append(requestContext.HttpContext.Request.UserHostAddress + "|");
            message.Append("Controller=");
            message.Append(requestContext.RouteData.GetRequiredString("controller") + "|");
            message.Append("Action=");
            message.Append(requestContext.RouteData.GetRequiredString("action") + "|");
            message.Append("StartTime=");
            message.Append(this.start_Time.ToString("yyyy-MM-dd HH:mm:ss") + "|");

            //get request parameters

            message.Append("RequestData=");
            message.Append(this.requestData + "|");

            message.Append("EndTime=");
            message.Append(this.end_Time.ToString("yyyy-MM-dd HH:mm:ss") + "|");

            message.Append("responseData=");
            message.Append(responseData + "|");

            message.Append("Message=");
            message.Append(des + "|");
            return message;
        }

        private void ProcessRequest(RequestContext requestContext)
        {
            this.start_Time = DateTime.Now;
            var response = requestContext.HttpContext.Response;
            try
            {

                var request = requestContext.HttpContext.Request;
                var server = requestContext.HttpContext.Server;
                //set cookie
                var rSessionCookie = new HttpCookie("myOwn", "test");
                response.Cookies.Add(rSessionCookie);
                //request Data
                this.requestData = ReadContext(requestContext.HttpContext.Request);

                //service
                IService serviceInstance = this.CreateServiceInstance(requestContext);
                string serviceMethodName = requestContext.RouteData.GetRequiredString("action");
                MethodInfo method =
                    serviceInstance.GetType()
                        .GetMethods()
                        .First(m => string.Compare(serviceMethodName, m.Name, true) == 0);
                List<object> parameters = new List<object>();
                foreach (ParameterInfo parameter in method.GetParameters())
                {
                    var paraObject = BindPrameter.ParameterBind(requestContext, parameter.Name, parameter.ParameterType);
                    //check if model is valid
                    IEnumerable<ValidationResult> errors =
                        (paraObject as IValidatableObject).Validate(new ValidationContext(paraObject, null, null));
                    if (errors.Count() > 0)
                    {
                        response.Clear();
                        response.ContentType = "application/json";
                        var errorResult = this.ExcuteErrorResult(errors.First().MemberNames.First(),
                            ResponseStatus.ParamError, errors.First().ErrorMessage);
                        response.Write(errorResult);
                        this.end_Time = DateTime.Now;
                        var stringbuilder = this.SetLogMessage(requestContext, (int)ResponseStatus.ParamError,
                            errorResult, "参数错误");
                        logger.Info(stringbuilder.ToString());
                        return;
                    }
                    parameters.Add(paraObject);
                }
                IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter();
                dateTimeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                string jsonResult =
                    JsonConvert.SerializeObject(
                        new JsonResultExtForPC(method.Invoke(serviceInstance, parameters.ToArray())).Data,
                        new JsonConverter[1]
                        {
                            (JsonConverter) dateTimeConverter
                        });

                response.Clear();
                response.ContentType = "application/json";

                response.Write(jsonResult);
                var stringMessage = this.SetLogMessage(requestContext, (int)ResponseStatus.Success,
                    jsonResult, "success");
                logger.Info(stringMessage.ToString());
            }
            catch (Exception ex)
            {
                response.Clear();
                response.ContentType = "application/json";
                var errorResult = this.ExcuteErrorResult("JsonHandler", ResponseStatus.ServerError, ex.Message);
                response.Write(errorResult);
                var logMessage = this.SetLogMessage(requestContext, (int)ResponseStatus.ServerError,
                    errorResult, ex.Message);
                logger.Error(ex, logMessage.ToString());


            }
            finally
            {
                response.End();
            }

        }

        public String ExcuteErrorResult(string key, ResponseStatus status, string message)
        {
            var header = new PCHeaderModel();
            header.Key = key;
            header.RspStatus = (int)status;
            header.RspDesc = message;
            var pcResponse = new PCResponseModel();
            pcResponse.header = header;
            pcResponse.body = null;
            return JsonConvert.SerializeObject(pcResponse);
        }

        public void ProcessRequest(HttpContext context)
        {

        }
        public bool IsReusable
        {
            get { return false; }
        }
        public RequestContext RequestContext { get; private set; }
    }
}