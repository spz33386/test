using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http.Filters;
using System.Web.Mvc;
using MvcApplication3.Common.Extension;
using MvcApplication3.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using System.Text;
using ActionFilterAttribute = System.Web.Mvc.ActionFilterAttribute;
using IActionFilter = System.Web.Mvc.IActionFilter;
using MvcApplication3.Common;

namespace MvcApplication3.Filters
{
    public class LogInfoMessage : ActionFilterAttribute, IActionFilter
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
        /// 当前请求URL
        /// </summary>
        private int status;

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
        /// 请求开始时间
        /// </summary>
        private String start_Time;

        /// <summary>
        /// 请求结束时间
        /// </summary>
        private String end_Time;

        private string ellapsedTime;
        /// <summary>
        /// 请求参数
        /// </summary>
        private IDictionary<string, object> requestData;

        /// <summary>
        /// 返回给View的参数
        /// 类型jsonString
        /// </summary>
        private String responseData;

        /// <summary>
        /// exception信息
        /// </summary>
        private String errorMessage;

        private StringBuilder message;

        private LogLevel loglevel;

        /// <summary>
        /// 信息的格式整理
        /// </summary>
        public void GetMessage()
        {
            this.message = new StringBuilder();
            this.message.Append("URL=");
            this.message.Append(this.url + "|");
            this.message.Append("Status=");
            this.message.Append(this.status + "|");
            this.message.Append("Elapsed time= ");
            this.message.Append(this.ellapsedTime + "|");
            this.message.Append("IP=");
            this.message.Append(this.IP + "|");
            this.message.Append("Controller=");
            this.message.Append(this.controller + "|");
            this.message.Append("Action=");
            this.message.Append(this.action + "|");
            this.message.Append("StartTime=");
            this.message.Append(this.start_Time + "|");
            if (this.requestData != null)
            {
                var data = JsonConvert.SerializeObject(this.requestData.Values);
                this.message.Append("RequestData=");
                this.message.Append(data + "|");
            }
            this.message.Append("EndTime=");
            this.message.Append(this.end_Time + "|");
            this.message.Append("ResponseData=");
            this.message.Append(this.end_Time + "|");
            if (this.responseData != null)
            {
                this.message.Append("responseData=");
                this.message.Append(this.responseData + "|");
            }
            this.message.Append("Message=");
            this.message.Append(this.errorMessage + "|");

        }

        /// <summary>
        /// Nlog 对象
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.timingEnabled)
            {
                this.timer = new Stopwatch();
                this.timer.Start();
                this.url = filterContext.HttpContext.Request.Url == null ? null : filterContext.HttpContext.Request.Url.ToString();
                this.controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                this.action = filterContext.ActionDescriptor.ActionName;
                this.IP = filterContext.HttpContext.Request.UserHostAddress;
                this.start_Time = filterContext.HttpContext.Timestamp.ToString(CultureInfo.InvariantCulture);
                this.requestData = filterContext.ActionParameters;

                var modelStateDic = filterContext.Controller.ViewData.ModelState;
                if (!modelStateDic.IsValid)
                {
                    var errorModel =
                       (from x in modelStateDic.Keys
                        where modelStateDic[x].Errors.Count > 0
                        select new PCHeaderModel()
                        {
                            RspStatus = (int)ResponseStatus.ParamError,
                            Key = x,
                            RspDesc = modelStateDic[x].Errors.Select(y => y.ErrorMessage).First()
                        }).First();
                    var PcHeaderModel = (PCHeaderModel)errorModel;
                    this.status = PcHeaderModel.RspStatus;
                    this.errorMessage = PcHeaderModel.RspDesc;
                    this.loglevel = LogLevel.Info;
                    this.timer.Stop();
                    this.end_Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    this.ellapsedTime = string.Format(CultureInfo.InvariantCulture, "{0}ms |", this.timer.ElapsedMilliseconds);
                    IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter();
                    dateTimeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    this.responseData = JsonConvert.SerializeObject(new JsonResultExtForPC(null, modelStateDic), new JsonConverter[1]
                        {
                          (JsonConverter)dateTimeConverter
                        });
                    this.GetMessage();
                    logger.Log(this.loglevel, this.message);
                    filterContext.Result = new JsonResultExtForPC(null, modelStateDic);
                }

            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            if (this.timingEnabled)
            {
                this.timer.Stop();
                this.ellapsedTime = string.Format(CultureInfo.InvariantCulture, "{0}ms |", this.timer.ElapsedMilliseconds);
                this.end_Time = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                this.loglevel = LogLevel.Info;
                this.status = 200;
                if (filterContext.Exception != null)
                {
                    this.errorMessage = filterContext.Exception.Message;
                    this.status = (int)ResponseStatus.ServerError;
                    this.loglevel = LogLevel.Error;
                }else if (filterContext.Result is ViewResult)
                {
                    this.responseData = JsonConvert.SerializeObject(((ViewResult)filterContext.Result).ViewData.Model);
                    ((ViewResult)filterContext.Result).ViewData["ExecutionTime"] = this.timer.ElapsedMilliseconds;

                }
                else if (filterContext.Result is JsonResult)
                {
                    this.responseData = JsonConvert.SerializeObject(((System.Web.Mvc.JsonResult)(filterContext.Result)).Data);
                    this.loglevel = this.status == 200 ? LogLevel.Info : LogLevel.Warn;
                    var data = ((System.Web.Mvc.JsonResult)(filterContext.Result)).Data;
                    string resMessage;

                    bool validateResult = (data as BaseViewModel).Validate(out resMessage);
                    if (!validateResult)
                    {
                        this.errorMessage = resMessage;
                        this.loglevel = LogLevel.Info;
                    }
                    filterContext.Result = new JsonResultExtForPC(((System.Web.Mvc.JsonResult)(filterContext.Result)).Data);
                }
                
                this.GetMessage();
                logger.Log(this.loglevel, this.message.ToString());
                

            }
        }
    }
}