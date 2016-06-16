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
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class JsonExceptionAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
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