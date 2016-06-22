using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication3.ViewModel
{
    public class OrderView : BaseViewModel
    {
        public string ClientName { get; set; }

        public override bool Validate(out string message)
        {
            if (this.ClientName == null)
            {
                message = string.Format("【查无结果】酒店具体信息房型列表,url:{0}", HttpContext.Current.Request.Url);
                return false;
            }
            message = "success";
            return true;
        }
    }
}