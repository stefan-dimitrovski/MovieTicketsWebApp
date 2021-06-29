using MovieShop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieShop.Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public MovieShopApplicationUser User { get; set; }
        public virtual ICollection<TicketInOrder> Tickets { get; set; }
    }
}
