using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly ApplicationDbContext context;

        public FavoriteService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public Favorite Add(string productId, string username)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == productId && p.IsRemoved != true);
            var user = GetUser(username);

            if (product is null || user is null)
            {
                return null;
            }

            var favorite = context.Favorites
                .Include(f => f.Product)
                .FirstOrDefault(f => f.ProductId == productId && f.UserId == user.Id);

            if (favorite is not null)
            {
                return favorite;
            }

            favorite = new Favorite()
            {
                ProductId = productId,
                UserId = user.Id,
                CreatedOn = DateTime.UtcNow
            };

            context.Favorites.Add(favorite);
            context.SaveChanges();

            return context.Favorites
                .Include(f => f.Product)
                .FirstOrDefault(f => f.Id == favorite.Id);
        }

        public bool Remove(string productId, string username)
        {
            var user = GetUser(username);

            if (user is null)
            {
                return false;
            }

            var favorite = context.Favorites
                .FirstOrDefault(f => f.ProductId == productId && f.UserId == user.Id);

            if (favorite is null)
            {
                return false;
            }

            context.Favorites.Remove(favorite);
            return context.SaveChanges() != 0;
        }

        public bool Clear(string username)
        {
            var user = GetUser(username);

            if (user is null)
            {
                return false;
            }

            var favorites = context.Favorites
                .Where(f => f.UserId == user.Id)
                .ToList();

            if (favorites.Count == 0)
            {
                return true;
            }

            context.Favorites.RemoveRange(favorites);
            return context.SaveChanges() != 0;
        }

        public ICollection<Favorite> GetByUsername(string username)
        {
            var user = GetUser(username);

            if (user is null)
            {
                return new List<Favorite>();
            }

            return context.Favorites
                .Include(f => f.Product)
                .Where(f => f.UserId == user.Id && f.Product.IsRemoved != true)
                .OrderByDescending(f => f.CreatedOn)
                .ToList();
        }

        public int GetCount(string username)
        {
            var user = GetUser(username);

            if (user is null)
            {
                return 0;
            }

            return context.Favorites
                .Count(f => f.UserId == user.Id && f.Product.IsRemoved != true);
        }

        public bool IsFavorite(string productId, string username)
        {
            var user = GetUser(username);

            if (user is null)
            {
                return false;
            }

            return context.Favorites
                .Any(f => f.ProductId == productId && f.UserId == user.Id && f.Product.IsRemoved != true);
        }

        private AppUser GetUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            return context.Users.FirstOrDefault(u => u.UserName == username);
        }
    }
}
