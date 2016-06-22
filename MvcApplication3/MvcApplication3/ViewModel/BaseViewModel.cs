using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication3.ViewModel
{
    public abstract class BaseViewModel
    {
        public abstract bool Validate(out string message);
    }
}