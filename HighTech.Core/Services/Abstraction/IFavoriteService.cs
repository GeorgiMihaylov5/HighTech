using HighTech.Core.Entities;

namespace HighTech.Core.Services.Abstraction
{
	public interface IFavoriteService
	{
		ICollection<FavoriteProduct> GetUserFavorites(string? userId);
	FavoriteProduct? AddFavorite(string? userId, string? productId);
		bool RemoveFavorite(string? userId, string? productId);
		bool ToggleFavorite(string? userId, string? productId);
		bool IsFavorite(string? userId, string? productId);
		int GetFavoritesCount(string? userId);
	}
}
