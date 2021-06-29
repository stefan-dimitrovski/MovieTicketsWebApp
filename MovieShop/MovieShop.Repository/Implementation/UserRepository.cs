using Microsoft.EntityFrameworkCore;
using MovieShop.Domain.Identity;
using MovieShop.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieShop.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<MovieShopApplicationUser> entities;
        string errorMessage = string.Empty;

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<MovieShopApplicationUser>();
        }
        public IEnumerable<MovieShopApplicationUser> GetAll()
        {
            return entities.AsEnumerable();
        }

        public MovieShopApplicationUser Get(string id)
        {
            return entities
                .Include(z => z.UserCart)
                .Include("UserCart.TicketInShoppingCarts")
                .Include("UserCart.TicketInShoppingCarts.Ticket")
                .SingleOrDefault(s => s.Id == id);
        }
        public void Insert(MovieShopApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(MovieShopApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }

        public void Delete(MovieShopApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }
    }
}
