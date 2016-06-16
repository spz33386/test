using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication3.ViewModel
{
    /// <summary>
    /// PC响应header实体
    /// </summary>
    public class PCHeaderModel
    {
        /// <summary>
        /// 状态码（由TCGHotelBase.MvcExtension.ResponseStatus枚举值转换）
        /// </summary>
        public int RspStatus { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string RspDesc { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Key { get; set; }
    }
}