using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using HighTech.Core.Services.Abstraction;

namespace HighTech.Core.Services
{
	public class FavoriteService : IFavoriteService
	{
		private readonly IFavoriteRepository favoriteRepository;

		public FavoriteService(IFavoriteRepository _favoriteRepository)
		{
			favoriteRepository = _favoriteRepository;
		}

		public ICollection<FavoriteProduct> GetUserFavorites(string? userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentException("User ID is required.", nameof(userId));
			}

			return favoriteRepository.GetUserFavorites(userId);
		}

		public FavoriteProduct? AddFavorite(string? userId, string? productId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentException("User ID is required.", nameof(userId));
			}

			if (string.IsNullOrWhiteSpace(productId))
			{
				throw new ArgumentException("Product ID is required.", nameof(productId));
			}

			return favoriteRepository.AddFavorite(userId, productId);
		}

		public bool RemoveFavorite(string? userId, string? productId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentException("User ID is required.", nameof(userId));
			}

			if (string.IsNullOrWhiteSpace(productId))
			{
				throw new ArgumentException("Product ID is required.", nameof(productId));
			}

			return favoriteRepository.RemoveFavorite(userId, productId);
		}

		public bool ToggleFavorite(string? userId, string? productId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentException("User ID is required.", nameof(userId));
			}

			if (string.IsNullOrWhiteSpace(productId))
			{
				throw new ArgumentException("Product ID is required.", nameof(productId));
			}

			if (favoriteRepository.IsFavorite(userId, productId))
			{
				favoriteRepository.RemoveFavorite(userId, productId);
				return false; // Removed from favorites
			}
			else
			{
				var result = favoriteRepository.AddFavorite(userId, productId);
				return result != null; // Added to favorites
			}
		}

		public bool IsFavorite(string? userId, string? productId)
		{
			if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(productId))
			{
				return false;
			}

			return favoriteRepository.IsFavorite(userId, productId);
		}

		public int GetFavoritesCount(string? userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				return 0;
			}

			return favoriteRepository.GetFavoritesCount(userId);
		}
	}
}
