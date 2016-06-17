using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using MvcApplication3.MappingProfile;
using MvcApplication3.Models;
using MvcApplication3.ParaModel;
using MvcApplication3.ViewModel;

namespace MvcApplication3.Service
{
    public class OrderQueryService:IService
    {
        public void Execute(RequestContext requestContext)
        {

        }

        public OrderView OrderbyId()
        {
            OrderView orderView = null;
            try
            {
                var order = new Order();
                order.Date = DateTime.Today;
                order.ClientName = "xiao song";
                orderView = OrderViewModelBuilder.BuildFromDomain(order);
            }
            catch (Exception e)
            {
                //log 记录
                return null;
            }
            return orderView;
        }
    }
}