using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NLog;

namespace MvcApplication3
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }
        protected void Application_Error(object sender, EventArgs e)
        {

            var objErr = Server.GetLastError().GetBaseException();
            var error = "发生异常页: " + Request.Url + "\r\n";
            error += "异常信息: " + objErr.Message;
            var logger = LogManager.GetCurrentClassLogger();
            logger.Log(LogLevel.Error, objErr, string.Format("全局异常,url:{0}", Request.Url));

        }
    }
}