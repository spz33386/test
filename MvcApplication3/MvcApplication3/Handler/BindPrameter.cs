using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Routing;

namespace MvcApplication3.Handler
{
    public class BindPrameter
    {
        public static Object ParameterBind(RequestContext requestContext, string modelName, Type modelType)
        {
            if (modelType.IsValueType || typeof(string) == modelType)
            {
                object instance;
                if (GetValueTypeInstance(requestContext, modelName, modelType, out instance))
                {
                    return instance;
                }
                return Activator.CreateInstance(modelType);
            }
            object modelInstance = Activator.CreateInstance(modelType);
            foreach (PropertyInfo property in modelType.GetProperties())
            {
                if (!property.CanWrite || (!property.PropertyType.IsValueType && property.PropertyType != typeof(string)))
                {
                    continue;
                }
                object propertyValue;
                if (GetValueTypeInstance(requestContext, property.Name, property.PropertyType, out propertyValue))
                {
                    property.SetValue(modelInstance, propertyValue, null);
                }
            }          
            return modelInstance;
        }       

        private static bool GetValueTypeInstance(RequestContext requestContext, string modelName, Type modelType, out object value)
        {
            var form = HttpContext.Current.Request.Form;
            string key;
            if (null != form)
            {
                key = form.AllKeys.FirstOrDefault(k => string.Compare(k, modelName, true) == 0);
                if (key != null)
                {
                    value = Convert.ChangeType(form[key], modelType);
                    return true;
                }
            }

            key = requestContext.RouteData.Values
                .Where(item => string.Compare(item.Key, modelName, true) == 0)
                .Select(item => item.Key).FirstOrDefault();
            if (null != key)
            {
                value = Convert.ChangeType(requestContext.RouteData.Values[key], modelType);
                return true;
            }

            key = requestContext.RouteData.DataTokens
                .Where(item => string.Compare(item.Key, modelName, true) == 0)
                .Select(item => item.Key).FirstOrDefault();
            if (null != key)
            {
                value = Convert.ChangeType(requestContext.RouteData.DataTokens[key], modelType);
                return true;
            }
            value = null;
            return false;
        }
    }
}