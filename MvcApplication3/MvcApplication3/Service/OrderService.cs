using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcApplication3.Models;
using MvcApplication3.ParaModel;
using MvcApplication3.ViewModel;
using MvcApplication3.MappingProfile;

namespace MvcApplication3.Service
{
    public class OrderService
    {
        public OrderView getOrderbyID(OrderParaModel orderParaModel)
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
            var y = 0;
            var x = 3 / y;
            return orderView;
        }
    }
}