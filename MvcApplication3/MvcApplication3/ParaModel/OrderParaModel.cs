using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcApplication3.ParaModel
{
    public class OrderParaModel:IValidatableObject
    {
        [DisplayName("产品名称")]
        public string ClientName { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("日期")]
        public DateTime Date { get; set; }
        public bool TermsAccepted { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            //获取displayName 属性以供错误信息使用
            //Type type = typeof(Product);
            //EntityAttribute entityAttribute = new EntityAttribute(type);
            //PropertyInfo[] PropertyInfos = type.GetProperties();
            //string shippingDisplayName = entityAttribute.GetDisplayAttributeName("Shipping");
            //string inventoryDisplayName = entityAttribute.GetDisplayAttributeName("Inventory");
            //return new ValidationResult(string.Format("{0}数量不能大于{1}数量", shippingDisplayName, inventoryDisplayName), new string[] { "Shipping" });
            if (string.IsNullOrEmpty(this.ClientName))
            {
                errors.Add(new ValidationResult("Please enter your name", new string[] { "ClientName" }));
            }
            if (DateTime.Now > this.Date)
            {
                errors.Add(new ValidationResult("Please enter a date in the future", new string[] { "Date" }));
            }
            if (errors.Count == 0 && this.ClientName == "Joe" && this.Date.DayOfWeek == DayOfWeek.Monday)
            {
                errors.Add(new ValidationResult("Joe cannot book appointments on Mondays", new string[] { "Date" }));
            }
            if (!this.TermsAccepted)
            {
                errors.Add(new ValidationResult("You must accept the terms"));
            }
            return errors;
        }
    }
}