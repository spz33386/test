using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace MvcApplication3.Service
{
    public interface IService
    {
        /// <summary>
        /// 执行指定的请求上下文。
        /// </summary>
        /// <param name="requestContext">请求上下文。</param>
        void Execute(RequestContext requestContext);
    }
}
