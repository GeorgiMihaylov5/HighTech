using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Infrastructure.Repositories
{
	public class FavoriteRepository : IFavoriteRepository
	{
		private readonly ApplicationDbContext context;

		public FavoriteRepository(ApplicationDbContext _context)
		{
			context = _context;
		}

		public ICollection<FavoriteProduct> GetUserFavorites(string? userId)
		{
			return context.FavoriteProducts
				.Where(f => f.UserId == userId)
				.Include(f => f.Product)
					.ThenInclude(p => p!.Category)
				.Include(f => f.Product)
					.ThenInclude(p => p!.ProductFieldValues!)
						.ThenInclude(pf => pf.Field)
				.OrderByDescending(f => f.CreatedAt)
				.ToList();
		}

		public FavoriteProduct? GetFavorite(string? userId, string? productId)
		{
			return context.FavoriteProducts
				.Include(f => f.Product)
				.FirstOrDefault(f => f.UserId == userId && f.ProductId == productId);
		}

		public FavoriteProduct? AddFavorite(string userId, string productId)
		{
			// Check if already exists
			var existing = context.FavoriteProducts
				.FirstOrDefault(f => f.UserId == userId && f.ProductId == productId);

			if (existing != null)
			{
				return existing;
			}

			// Verify product exists
			var product = context.Products.Find(productId);
			if (product == null || product.IsRemoved)
			{
				return null;
			}

			var favorite = new FavoriteProduct
			{
				UserId = userId,
				ProductId = productId,
				CreatedAt = DateTime.UtcNow
			};

			context.FavoriteProducts.Add(favorite);
			context.SaveChanges();

			return context.FavoriteProducts
				.Include(f => f.Product)
				.FirstOrDefault(f => f.Id == favorite.Id);
		}

		public bool RemoveFavorite(string? userId, string? productId)
		{
			var favorite = context.FavoriteProducts
				.FirstOrDefault(f => f.UserId == userId && f.ProductId == productId);

			if (favorite == null)
			{
				return false;
			}

			context.FavoriteProducts.Remove(favorite);
			context.SaveChanges();

			return true;
		}

		public bool IsFavorite(string? userId, string? productId)
		{
			return context.FavoriteProducts
				.Any(f => f.UserId == userId && f.ProductId == productId);
		}

		public int GetFavoritesCount(string? userId)
		{
			return context.FavoriteProducts
				.Count(f => f.UserId == userId);
		}
	}
}
