using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;
using MvcApplication3.Service;

namespace MvcApplication3.ServiceFactory
{
    public class ReflelctionServiceFactory
    {
        private static List<Type> controllerTypes;
        static ReflelctionServiceFactory()
        {
            controllerTypes = new List<Type>();
            foreach (Assembly assembly in BuildManager.GetReferencedAssemblies())
            {
                controllerTypes.AddRange(assembly.GetTypes().Where(type => typeof(IService).IsAssignableFrom(type)));
            }
        }

        public IService CreateService(RequestContext requestContext, string controllerName)
        {
            Type controllerType = this.GetServiceType(requestContext.RouteData, controllerName);
            if (null == controllerType)
            {
                throw new HttpException(404, "No controller found");
            }
            return (IService)Activator.CreateInstance(controllerType);
        }

        protected Type GetServiceType(RouteData routeData, string controllerName)
        {
            var types = controllerTypes.Where(type => string.Compare(controllerName, type.Name, true) == 0).ToArray();
            if (types.Length == 0)
            {
                return null;
            }

            //通过路由对象的命名空间进行匹配
            var namespaces = routeData.DataTokens["Namespaces"] as IEnumerable<string>;
            namespaces = namespaces ?? new string[0];
            Type contrllerType = this.GetControllerType(namespaces, types);
            if (null != contrllerType)
            {
                return contrllerType;
            }

            //default namespace
            namespaces = new String[1] { "MvcApplication3.Service" };
            contrllerType = this.GetControllerType(namespaces, types);
            if (null != contrllerType)
            {
                return contrllerType;
            }

            //如果只存在一个类型名称匹配的Controller，则返回之
            if (types.Length == 1)
            {
                return types[0];
            }

            //如果具有多个类型名称匹配的Controller，则抛出异常
            throw new InvalidOperationException("Multiple types were found that match the requested controller name.");
        }

        private static bool IsNamespaceMatch(string requestedNamespace, string targetNamespace)
        {
            if (!requestedNamespace.EndsWith(".*", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(requestedNamespace, targetNamespace, StringComparison.OrdinalIgnoreCase);
            }
            requestedNamespace = requestedNamespace.Substring(0, requestedNamespace.Length - ".*".Length);
            if (!targetNamespace.StartsWith(requestedNamespace, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return ((requestedNamespace.Length == targetNamespace.Length) || (targetNamespace[requestedNamespace.Length] == '.'));
        }

        private Type GetControllerType(IEnumerable<string> namespaces, Type[] controllerTypes)
        {
            var types = (from type in controllerTypes
                         where namespaces.Any(ns => IsNamespaceMatch(ns, type.Namespace))
                         select type).ToArray();
            switch (types.Length)
            {
                case 0: return null;
                case 1: return types[0];
                default: throw new InvalidOperationException("Multiple types were found that match the requested controller name.");
            }
        }
    }
}