using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieShop.Domain.DomainModels
{
    public class Ticket : BaseEntity
    {
        [Required]
        public string Movie { get; set; }
        [Required]
        [Display(Name ="Date")]
        public DateTime ValidTime { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public Genre Genre { get; set; }
        public virtual ICollection<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public virtual ICollection<TicketInOrder> Orders { get; set; }

    }
}
