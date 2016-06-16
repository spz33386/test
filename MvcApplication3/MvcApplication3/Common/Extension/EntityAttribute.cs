using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MvcApplication3.Common.Extension
{
    public class EntityAttribute
    {
        private Type type;
        public EntityAttribute(Type type)
        {
            this.type = type;
        }
        public string GetDisplayAttributeName(string propertyName)
        {
            var propertyInfo = type.GetProperty(propertyName);
            object[] attrs = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            return (attrs[0] as DisplayNameAttribute).DisplayName;
        }
    }
}