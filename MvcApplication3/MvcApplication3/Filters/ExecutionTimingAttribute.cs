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
                this.dateTime = filterContext.HttpContext.Timestamp.ToString(CultureInfo.InvariantCulture);
                this.requestData = filterContext.ActionParameters;
               
                this.GetMessage();

                logger.Log(LogLevel.Info, this.mesge.ToString());
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
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
    }
}