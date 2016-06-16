using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;

namespace MvcApplication3.Filters
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class LogExceptionAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                var controllerName = (string)filterContext.RouteData.Values["controller"];
                var actionName = (string)filterContext.RouteData.Values["action"];
                var msgTemplate = "在执行 controller[{0}] 的 action[{1}] 时产生异常";
                LogManager.GetLogger("LogExceptionAttribute").Error(filterContext.Exception, string.Format(msgTemplate, controllerName, actionName));
            }

            // 否则调用原始设置
            base.OnException(filterContext);
        }
    }
}