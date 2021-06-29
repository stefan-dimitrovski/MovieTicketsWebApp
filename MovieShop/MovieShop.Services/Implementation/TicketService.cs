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
    public class TicketService : ITicketService
    {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<TicketInShoppingCart> _ticketInShoppingCartRepository;
        private readonly IUserRepository _userRepository;
        
        public TicketService(IRepository<Ticket> ticketRepository, IUserRepository userRepository, IRepository<TicketInShoppingCart> ticketInShoppingCartRepository)
        {
            _ticketRepository = ticketRepository;
            _ticketInShoppingCartRepository = ticketInShoppingCartRepository;
            _userRepository = userRepository;
        }    

        public void CreateNewTicket(Ticket t)
        {
            this._ticketRepository.Insert(t);
        }

        public void DeleteTicket(Guid id)
        {
            var ticket = this.GetDetailsForTicket(id);
            this._ticketRepository.Delete(ticket);
        }

        public List<Ticket> GetAllTickets()
        {
            return this._ticketRepository.GetAll().ToList();
        }

        public Ticket GetDetailsForTicket(Guid? id)
        {
            return this._ticketRepository.Get(id);
        }

        public void UpdeteExistingTicket(Ticket t)
        {
            this._ticketRepository.Update(t);
        }

        public bool AddToShoppingCart(AddToShoppingCartDto item, string userID)
        {

            var user = this._userRepository.Get(userID);

            var userShoppingCart = user.UserCart;

            if (item.TicketId != null && userShoppingCart != null)
            {
                var ticket = this.GetDetailsForTicket(item.TicketId);

                if (ticket != null)
                {
                    TicketInShoppingCart itemToAdd = new TicketInShoppingCart
                    {
                        Ticket = ticket,
                        TicketId = ticket.Id,
                        ShoppingCart = userShoppingCart,
                        ShoppingCartId = userShoppingCart.Id,
                        Quantity = item.Quantity
                    };

                    this._ticketInShoppingCartRepository.Insert(itemToAdd);
                    return true;
                }
                return false;
            }
            return false;
        }

        public AddToShoppingCartDto GetShoppingCartInfo(Guid? id)
        {

            var ticket = this.GetDetailsForTicket(id);
            AddToShoppingCartDto model = new AddToShoppingCartDto
            {
                SelectedTicket = ticket,
                TicketId = ticket.Id,
                Quantity = 1
            };

            return model;
        }

        public List<Ticket> GetValidTickets()
        {
            var today = DateTime.Now;

            var tickets = this._ticketRepository.GetAll();

            var validTickets = tickets.Where(z => z.ValidTime >= today).OrderBy(x => x.ValidTime).ToList();

            return validTickets;

        }
    }
}
