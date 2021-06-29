using MovieShop.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieShop.Services.Interface
{
    public interface IShoppingCartService
    {
        ShoppingCartDto getShoppingCartInfo(string userId);
        bool deleteTicketFromShoppingCart(string userId, Guid Id);
        bool orderNow(string userId);
    }
}
