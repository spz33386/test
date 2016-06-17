using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace MvcApplication3.Handler
{
    public class CustomeRouteHandler : IRouteHandler
    {
        private IControllerFactory _controllerFactory;
        public CustomeRouteHandler()
            : this(ControllerBuilder.Current.GetControllerFactory())
        { }
        public CustomeRouteHandler(IControllerFactory controllerFactory)
        {
            _controllerFactory = controllerFactory;
        }
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string controllerName = (string)requestContext.RouteData.GetRequiredString("controller");
            SessionStateBehavior sessionStateBehavior = _controllerFactory.GetControllerSessionBehavior(requestContext, controllerName);
            requestContext.HttpContext.SetSessionStateBehavior(sessionStateBehavior);
            return new JsonHandler(requestContext);
        }
    }
}