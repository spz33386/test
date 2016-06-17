using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using MvcApplication3.Common;
using MvcApplication3.Models;
using MvcApplication3.Service;
using MvcApplication3.ServiceFactory;
using Newtonsoft.Json;


namespace MvcApplication3.Handler
{
    public class JsonHandler : IHttpHandler
    {
        public JsonHandler(RequestContext context)
        {
            ProcessRequest(context);
        }

        private string ReadContext(HttpRequestBase request)
        {
            try
            {
                using (var stream = request.InputStream)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        string body = sr.ReadToEnd();
                        return body;
                    }
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private IService CreateServiceInstance(RequestContext requestContext)
        {
            var serviceFactory = new ReflelctionServiceFactory();
            string serviceName = requestContext.RouteData.GetRequiredString("controller");
            return serviceFactory.CreateService(requestContext, serviceName);
        }

        private void ProcessRequest(RequestContext requestContext)
        {
            var response = requestContext.HttpContext.Response;
            var request = requestContext.HttpContext.Request;
            var server = requestContext.HttpContext.Server;

            //request Data
            var data = string.Empty;
            data = ReadContext(requestContext.HttpContext.Request);

            //service
            IService serviceInstance = this.CreateServiceInstance(requestContext);
            string serviceMethodName = requestContext.RouteData.GetRequiredString("action");
            MethodInfo method = serviceInstance.GetType().GetMethods().First(m => string.Compare(serviceMethodName, m.Name, true) == 0);
            List<object> parameters = new List<object>();
           foreach (ParameterInfo parameter in method.GetParameters())
           {
               
           }
            string jsonResult = JsonConvert.SerializeObject(new JsonResultExtForPC(method.Invoke(serviceInstance, null)).Data);

            response.Clear();
            response.ContentType = "application/json";
                       
            response.Write(jsonResult);
            //write some code
            response.End();
        }
        public void ProcessRequest(HttpContext context)
        {

        }
        public bool IsReusable
        {
            get { return false; }
        }
        public RequestContext RequestContext { get; private set; }
    }
}