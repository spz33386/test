using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication3.Common.Extension;
using MvcApplication3.ViewModel;
using NLog;


namespace MvcApplication3.Filters
{
   
    public class JsonAttribute : HandleErrorAttribute
    {       

        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                //返回异常JSON
                var pcResponseModel = new PCResponseModel();
                pcResponseModel.header = new PCHeaderModel() { RspStatus = (int)ResponseStatus.ServerError, Key = "程序发生异常错误", RspDesc = filterContext.Exception.Message };
                pcResponseModel.body = null;
                filterContext.ExceptionHandled = true;
                filterContext.Result = new JsonResult
                {
                    Data = pcResponseModel,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}