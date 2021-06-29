using MovieShop.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieShop.Domain.DTO
{
    public class ShoppingCartDto
    {
        public List<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public double TotalPrice { get; set; }
    }
}
