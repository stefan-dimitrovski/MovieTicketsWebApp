using MovieShop.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieShop.Services.Interface
{
    public interface IOrderService
    {
        List<Order> GetAllOrders(string userID);

        Order GetOrderDetails(BaseEntity model);
    }
}
