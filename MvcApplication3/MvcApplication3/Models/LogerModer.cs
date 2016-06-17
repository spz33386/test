using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace MvcApplication3.Models
{
    public class LogerModer
    {
        /// <summary>
        /// 是否开启时间记录功能
        /// </summary>
        private bool timingEnabled = bool.Parse(ConfigurationManager.AppSettings["TimingEnabled"]);

        /// <summary>
        /// 时间记录
        /// </summary>
        public Stopwatch timer;

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

        /// <summary>
        /// 请求参数
        /// </summary>
        private IDictionary<string, object> requestData;

        /// <summary>
        /// 返回给View的参数
        /// </summary>
        private IDictionary<string, object> responseData;

        /// <summary>
        /// exception信息
        /// </summary>
        private String errorMessage;

        /// <summary>
        /// 信息的格式整理
        /// </summary>
        public StringBuilder GetMessage()
        {
            var message = new StringBuilder();
            message.Append("URL=");
            message.Append(this.url + "|");
            message.Append("Status=");
            message.Append(this.status + "|");
            message.Append("IP=");
            message.Append(this.IP + "|");
            message.Append("Controller=");
            message.Append(this.controller + "|");
            message.Append("Action=");
            message.Append(this.action + "|");
            message.Append("StartTime=");
            message.Append(this.start_Time + "|");
            if (this.requestData != null)
            {
                var data = JsonConvert.SerializeObject(this.requestData.Values);
                message.Append("RequestData=");
                message.Append(data + "|");
            }
            message.Append("EndTime=");
            message.Append(this.end_Time + "|");
            message.Append("ResponseData=");
            message.Append(this.end_Time + "|");
            if (this.responseData != null)
            {
                var data = JsonConvert.SerializeObject(this.responseData.Values);
                message.Append("responseData=");
                message.Append(data + "|");
            }
            message.Append("Message=");
            message.Append(this.errorMessage + "|");
            return message;
        }
    }
}