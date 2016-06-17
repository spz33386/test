using MvcApplication3.Common.Extension;
using MvcApplication3.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication3.Common
{
    public class JsonResultExtForPC:JsonResult
    {
        private string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public PCHeaderModel PcHeaderModel { get; set; }

        public JsonResultExtForPC()
        {
        }

        public JsonResultExtForPC(object data, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet, string datetimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            var pcResponseModel = new PCResponseModel();
            this.PcHeaderModel = new PCHeaderModel() { RspStatus = 200, Key = null, RspDesc = null };
            pcResponseModel.header = this.PcHeaderModel;
            pcResponseModel.body = (object)data;
            this.Data = pcResponseModel;
            this.JsonRequestBehavior = behavior;
            this.dateTimeFormat = datetimeFormat;
        }

        public JsonResultExtForPC(object data, ModelStateDictionary modelStateDic, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet, string datetimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (modelStateDic != null)
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
                var pcResponseModel = new PCResponseModel();
                this.PcHeaderModel = (PCHeaderModel) errorModel;
                pcResponseModel.header = (PCHeaderModel)errorModel;
                pcResponseModel.body = null;
                this.Data = pcResponseModel;

            }
            else
            {
                var pcResponseModel = new PCResponseModel();
                pcResponseModel.header = new PCHeaderModel() { RspStatus = 200, Key = null, RspDesc = null };
                pcResponseModel.body = (object)data;
                this.Data = pcResponseModel;
            }
            this.JsonRequestBehavior = behavior;
            this.dateTimeFormat = datetimeFormat;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context上下文不能为null");
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("MvcResources.JsonRequest_GetNotAllowed");
            context.HttpContext.Response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;
            if (this.ContentEncoding != null)
                context.HttpContext.Response.ContentEncoding = this.ContentEncoding;
            IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter();
            dateTimeConverter.DateTimeFormat = this.dateTimeFormat;
           context.HttpContext.Response.Write(JsonConvert.SerializeObject(this.Data, new JsonConverter[1]
        {
          (JsonConverter)dateTimeConverter
        }));
        }
    }
}