using MovieShop.Domain.DomainModels;
using MovieShop.Domain.DTO;
using MovieShop.Repository.Interface;
using MovieShop.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieShop.Services.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<TicketInOrder> _ticketInOrderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMailService _mailService;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IUserRepository userRepository, IRepository<Order> orderRepository, IRepository<TicketInOrder> ticketInOrderRepository, IMailService mailService)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _ticketInOrderRepository = ticketInOrderRepository;
            _mailService = mailService;
        }

        public bool deleteTicketFromShoppingCart(string userId, Guid Id)
        {
            if (!string.IsNullOrEmpty(userId) && Id != null)
            {
                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.UserCart;

                var itemToDelete = userShoppingCart.TicketInShoppingCarts.Where(z => z.TicketId.Equals(Id)).FirstOrDefault();

                userShoppingCart.TicketInShoppingCarts.Remove(itemToDelete);

                this._shoppingCartRepository.Update(userShoppingCart);

                return true;
            }
            return false;
        }

        public ShoppingCartDto getShoppingCartInfo(string userId)
        {
            var loggedInUser = this._userRepository.Get(userId);

            var userShoppingCart = loggedInUser.UserCart;

            var allTickets = userShoppingCart.TicketInShoppingCarts.ToList();

            var allTicketPrice = allTickets.Select(z => new
            {
                TicketPrice = z.Ticket.Price,
                Quantity = z.Quantity

            }).ToList();

            double totalPrice = 0;

            foreach (var item in allTicketPrice)
            {
                totalPrice += item.TicketPrice * item.Quantity;
            }

            ShoppingCartDto scDto = new ShoppingCartDto
            {
                TicketInShoppingCarts = allTickets,
                TotalPrice = totalPrice
            };

            return scDto;
        }

        public bool orderNow(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.UserCart;

                MailRequest mail = new MailRequest();
                mail.ToEmail = loggedInUser.Email;
                mail.Subject = "Successfully Created Order";

                Order orderItem = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    User = loggedInUser
                };

                this._orderRepository.Insert(orderItem);

                List<TicketInOrder> ticketInOrders = new List<TicketInOrder>();

                var result = userShoppingCart.TicketInShoppingCarts.Select(z => new TicketInOrder
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderItem.Id,
                        TicketId = z.Ticket.Id,
                        SelectedTicket = z.Ticket,
                        UserOrder = orderItem,
                        Quantity = z.Quantity
                    }).ToList();

                StringBuilder sb = new StringBuilder();

                var totalPrice = 0.0;

                sb.AppendLine("Your order is complete. The order contains: ");

                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    totalPrice += item.Quantity * item.SelectedTicket.Price;

                    sb.AppendLine(i.ToString() + ". " + item.SelectedTicket.Movie + " with price of: " + item.SelectedTicket.Price + " and quantity of: " + item.Quantity);

                }

                sb.AppendLine("Total price: " + totalPrice.ToString());

                mail.Body = sb.ToString();

                ticketInOrders.AddRange(result);

                foreach (var item in ticketInOrders)
                {
                    this._ticketInOrderRepository.Insert(item);
                }

                loggedInUser.UserCart.TicketInShoppingCarts.Clear();

                this._userRepository.Update(loggedInUser);
                this._mailService.SendEmailAsync(mail);

                return true;
            }
            return false;
        }
    }
}
