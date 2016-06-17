using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcApplication3.Common;
using MvcApplication3.Common.Extension;
using MvcApplication3.ViewModel;
using Newtonsoft.Json;
using NLog;

namespace MvcApplication3.Controllers
{
    public class BaseController: Controller
    {
        /// <summary>
        /// 是否开启时间记录功能
        /// </summary>
        private bool timingEnabled = bool.Parse(ConfigurationManager.AppSettings["TimingEnabled"]);

        /// <summary>
        /// 时间记录
        /// </summary>
        private Stopwatch timer;

        /// <summary>
        /// 当前请求URL
        /// </summary>
        private String url;

        /// <summary>
        /// 请求controller
        /// </summary>
        private String controller;

        /// <summary>
        /// action方法
        /// </summary>
        private String action;

        /// <summary>
        /// client ip
        /// </summary>
        private String IP;

        /// <summary>
        /// 请求时间点
        /// </summary>
        private String dateTime;

        private IDictionary<string, object> requestData;

        /// <summary>
        /// 记录信息
        /// </summary>
        private StringBuilder mesge;

        private string responseData;

        /// <summary>
        /// 信息的格式整理
        /// </summary>
        public void GetMessage()
        {

            this.mesge = new StringBuilder();
            this.mesge.Append("URL=");
            this.mesge.Append(this.url + "|");
            this.mesge.Append("IP=");
            this.mesge.Append(this.IP + "|");
            this.mesge.Append("Controller=");
            this.mesge.Append(this.controller + "|");
            this.mesge.Append("Action=");
            this.mesge.Append(this.action + "|");
            this.mesge.Append("TimeStamp=");
            this.mesge.Append(this.dateTime + "|");

            if (this.requestData != null)
            {
                var data = JsonConvert.SerializeObject(this.requestData.Values);
                this.mesge.Append("RequestData=");
                this.mesge.Append(data + "|");
            }

        }

        /// <summary>
        /// Nlog 对象
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        /// <summary>
        /// 创建 <see cref="T:System.Web.Mvc.JsonResult"/> 对象，该对象使用内容类型、内容编码和 JSON 请求行为将指定对象序列化为 JavaScript 对象表示法 (JSON) 格式。
        /// </summary>
        /// 
        /// <returns>
        /// 将指定对象序列化为 JSON 格式的结果对象。
        /// </returns>
        /// <param name="data">要序列化的 JavaScript 对象图。</param><param name="contentType">内容类型（MIME 类型）。</param><param name="contentEncoding">内容编码。</param><param name="behavior">JSON 请求行为</param>
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.timingEnabled)
            {
                this.timer = new Stopwatch();
                this.timer.Start();
                this.url = filterContext.HttpContext.Request.Url == null ? null : filterContext.HttpContext.Request.Url.ToString();
                this.controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                this.action = filterContext.ActionDescriptor.ActionName;
                this.IP = filterContext.HttpContext.Request.UserHostAddress;
                this.dateTime = filterContext.HttpContext.Timestamp.ToString(CultureInfo.InvariantCulture);
                this.requestData = filterContext.ActionParameters;

                this.GetMessage();

                logger.Log(LogLevel.Info, this.mesge.ToString());
            }

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            if (this.timingEnabled)
            {
                this.timer.Stop();
                this.dateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                this.GetMessage();
                this.mesge.Append("ExectionTime=");
                this.mesge.Append(string.Format(CultureInfo.InvariantCulture, "{0}ms |", this.timer.ElapsedMilliseconds));
                if (filterContext.Result is ViewResult)
                {
                    this.responseData = JsonConvert.SerializeObject(((ViewResult)filterContext.Result).ViewData.Model);
                    ((ViewResult)filterContext.Result).ViewData["ExecutionTime"] = this.timer.ElapsedMilliseconds;
                }
                else if (filterContext.Result is JsonResult)
                {
                    this.responseData = JsonConvert.SerializeObject(((JsonResultExtForPC)filterContext.Result).Data);
                }
                this.mesge.Append("ResponseData=");
                this.mesge.Append(this.responseData + "|");
                logger.Log(LogLevel.Info, this.mesge.ToString());
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                //Get all the info we need to define where the error occured and with what data
                var param = new NameValueCollection { filterContext.HttpContext.Request.Form, filterContext.HttpContext.Request.QueryString };
                var controllerName = (string)filterContext.RouteData.Values["controller"];
                var actionName = (string)filterContext.RouteData.Values["action"];
                var msgTemplate = "在执行 controller[{0}] 的 action[{1}] 时产生异常{2}.请求参数为{3}";
                LogManager.GetLogger("JsonExceptionAttribute").Error(string.Format(msgTemplate, controllerName, actionName, filterContext.Exception.Message, param));

                //阻止后面的filter 继续处理
                filterContext.ExceptionHandled = true;

                //返回异常JSON
                var pcResponseModel = new PCResponseModel();
                pcResponseModel.header = new PCHeaderModel() { RspStatus = (int)ResponseStatus.ServerError, Key = "程序发生异常错误", RspDesc = filterContext.Exception.Message };
                pcResponseModel.body = null;

                filterContext.Result = new JsonResult
                {
                    Data = pcResponseModel,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}