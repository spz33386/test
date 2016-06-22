using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MvcApplication3.Filters;
using MvcApplication3.Models;
using MvcApplication3.ParaModel;
using MvcApplication3.Service;
using MvcApplication3.MappingProfile;
using MvcApplication3.ViewModel;
using MvcApplication3.Common;

namespace MvcApplication3.Controllers
{
    public class HomeController : Controller
    {
        private OrderService orderService = new OrderService();
        public ActionResult Index(int? id)
        {
            HttpContext.Session["sessionString"] = "myOwn";
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            var order = new Order()
            {
                ClientName = "sven",
                Date = DateTime.Today,
                TermsAccepted = true
            };
            return View(order);
        }
        [HttpGet]
        [Json]
        public JsonResult Submit(OrderParaModel orderParaModel)
        {            
            var model = orderService.getOrderbyID(orderParaModel);
            if (model != null)
            {
            }
            return Json(model, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Json]
        public ActionResult Submit2(OrderParaModel orderParaModel)
        {
            if (ModelState.IsValid)
            {
                return new JsonResultExtForPC(orderService.getOrderbyID(orderParaModel));
            }
            else
            {
                return new JsonResultExtForPC(null, ModelState, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult About(OrderParaModel orderParaModel)
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
