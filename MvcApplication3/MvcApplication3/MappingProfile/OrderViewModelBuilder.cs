using MvcApplication3.Models;
using MvcApplication3.ParaModel;
using MvcApplication3.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication3.MappingProfile
{
    public class OrderViewModelBuilder
    {
       
        public static OrderView BuildFromDomain(Order order)
        {
            var orderView = new OrderView();
            orderView.ClientName = order.ClientName;
            
            return orderView;
        }

        public static OrderView BuildFromParaModel(OrderParaModel orderParaModel)
        {
            var orderView = new OrderView();
            orderView.ClientName = orderParaModel.ClientName;
            return orderView;
        }
    }
}