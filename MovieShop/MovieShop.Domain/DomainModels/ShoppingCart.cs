using MovieShop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieShop.Domain.DomainModels
{
    public class ShoppingCart : BaseEntity
    {
        public string OwnerId { get; set; }
        public MovieShopApplicationUser Owner { get; set; }
        public virtual ICollection<TicketInShoppingCart> TicketInShoppingCarts{ get; set; }
    }
}
