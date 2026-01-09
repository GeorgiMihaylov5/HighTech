using HighTech.Core.Entities;
using HighTech.DTOs;

namespace HighTech.Mappers
{
	public static class FavoriteMapper
	{
		public static FavoriteProductDTO ToDTO(FavoriteProduct favorite)
		{
			if (favorite is null)
			{
				return null!;
			}

			return new FavoriteProductDTO()
			{
				Id = favorite.Id,
				UserId = favorite.UserId,
				ProductId = favorite.ProductId,
				Product = favorite.Product != null ? ProductMapper.ToDTO(favorite.Product) : null,
				CreatedAt = favorite.CreatedAt
			};
		}

		public static List<FavoriteProductDTO> ToDTOList(ICollection<FavoriteProduct> favorites)
		{
			return favorites?.Select(f => ToDTO(f)).ToList() ?? new List<FavoriteProductDTO>();
		}
	}
}
