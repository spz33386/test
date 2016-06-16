using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcApplication3.Models
{
    public class Order
    {
        public string ClientName { get; set; }
        public DateTime Date { get; set; }
        public bool TermsAccepted { get; set; }        
    }
}