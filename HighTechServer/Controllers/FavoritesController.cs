using HighTech.Core.Services.Abstraction;
using HighTech.Mappers;
using HighTechServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HighTech.Controllers
{
	[ApiController]
	[Route("[controller]/[action]")]
	public class FavoritesController : Controller
	{
		private readonly IFavoriteService favoriteService;

		public FavoritesController(IFavoriteService _favoriteService)
		{
			favoriteService = _favoriteService;
		}

		[Authorize]
		[HttpGet]
		public IActionResult GetUserFavorites()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Response.Error("User not authenticated.", ErrorCode.UnauthorizedAccess, 401);
			}

			var favorites = favoriteService.GetUserFavorites(userId);
			return Response.Success(FavoriteMapper.ToDTOList(favorites));
		}

		[Authorize]
		[HttpPost("{productId}")]
		public IActionResult AddFavorite(string productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return Response.Error("Product ID is required.", ErrorCode.FavoriteProductIdMissing, 400);
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Response.Error("User not authenticated.", ErrorCode.UnauthorizedAccess, 401);
			}

			var favorite = favoriteService.AddFavorite(userId, productId);

			if (favorite == null)
			{
				return Response.Error("Product not found or is no longer available.", ErrorCode.ProductNotFound, 404);
			}

			return Response.Success(new { message = "Product added to favorites.", favorite = FavoriteMapper.ToDTO(favorite) }, 201);
		}

		[Authorize]
		[HttpDelete("{productId}")]
		public IActionResult RemoveFavorite(string productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return Response.Error("Product ID is required.", ErrorCode.FavoriteProductIdMissing, 400);
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Response.Error("User not authenticated.", ErrorCode.UnauthorizedAccess, 401);
			}

			var removed = favoriteService.RemoveFavorite(userId, productId);

			if (!removed)
			{
				return Response.Error("Favorite not found.", ErrorCode.FavoriteNotFound, 404);
			}

			return Response.Success(new { message = "Product removed from favorites." });
		}

		[Authorize]
		[HttpPost("{productId}")]
		public IActionResult ToggleFavorite(string productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return Response.Error("Product ID is required.", ErrorCode.FavoriteProductIdMissing, 400);
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Response.Error("User not authenticated.", ErrorCode.UnauthorizedAccess, 401);
			}

			var isNowFavorite = favoriteService.ToggleFavorite(userId, productId);

			return Response.Success(new
			{
				message = isNowFavorite ? "Product added to favorites." : "Product removed from favorites.",
				isFavorite = isNowFavorite
			});
		}

		[Authorize]
		[HttpGet("{productId}")]
		public IActionResult IsFavorite(string productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return Response.Error("Product ID is required.", ErrorCode.FavoriteProductIdMissing, 400);
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Response.Error("User not authenticated.", ErrorCode.UnauthorizedAccess, 401);
			}

			var isFavorite = favoriteService.IsFavorite(userId, productId);

			return Response.Success(new { isFavorite });
		}

		[Authorize]
		[HttpGet]
		public IActionResult GetFavoritesCount()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Response.Error("User not authenticated.", ErrorCode.UnauthorizedAccess, 401);
			}

			var count = favoriteService.GetFavoritesCount(userId);

			return Response.Success(new { count });
		}
	}
}
