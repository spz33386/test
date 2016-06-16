using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication3.Common.Extension
{
    public enum ResponseStatus
    {
        Success = 200,
        ParamError = 400,
        UnauthorizedError = 401,
        Error404 = 404,
        NotSuport = 415,
        BusinessError = 416,
        ServerError = 500,
    }
}