using MovieShop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieShop.Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<MovieShopApplicationUser> GetAll();
        MovieShopApplicationUser Get(string id);
        void Insert(MovieShopApplicationUser entity);
        void Update(MovieShopApplicationUser entity);
        void Delete(MovieShopApplicationUser entity);
    }
}
