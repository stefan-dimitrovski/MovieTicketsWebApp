using Microsoft.AspNetCore.Identity;
using MovieShop.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieShop.Domain.Identity
{
    public class MovieShopApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ShoppingCart UserCart { get; set; }
    }
}
 