using MovieShop.Domain.DomainModels;
using MovieShop.Repository.Interface;
using MovieShop.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieShop.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<Order> GetAllOrders(string userId)
        {
            return _orderRepository.getAllOrders().Where(z => z.UserId.Equals(userId)).ToList();
        }

        public Order GetOrderDetails(BaseEntity model)
        {
            return this._orderRepository.getOrderDetails(model);
        }
    }
}
