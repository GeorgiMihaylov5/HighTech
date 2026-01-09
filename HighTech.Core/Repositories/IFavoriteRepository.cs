using HighTech.Core.Entities;

namespace HighTech.Core.Repositories
{
	public interface IFavoriteRepository
	{
		ICollection<FavoriteProduct> GetUserFavorites(string? userId);
		FavoriteProduct? GetFavorite(string? userId, string? productId);
		FavoriteProduct? AddFavorite(string userId, string productId);
		bool RemoveFavorite(string? userId, string? productId);
		bool IsFavorite(string? userId, string? productId);
		int GetFavoritesCount(string? userId);
	}
}
