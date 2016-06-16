using MvcApplication3.Filters;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication3
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogInfoMessage());
            filters.Add(new LogExceptionAttribute());
        }
    }
}