using HighTech.Core.Services.Abstraction;
using HighTech.Mappers;
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
			try
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User not authenticated.");
				}

				var favorites = favoriteService.GetUserFavorites(userId);
				return Json(FavoriteMapper.ToDTOList(favorites));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize]
		[HttpPost("{productId}")]
		public IActionResult AddFavorite(string productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return BadRequest("Product ID is required.");
			}

			try
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User not authenticated.");
				}

				var favorite = favoriteService.AddFavorite(userId, productId);

				if (favorite == null)
				{
					return NotFound("Product not found or is no longer available.");
				}

				return Json(new { message = "Product added to favorites.", favorite = FavoriteMapper.ToDTO(favorite) });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize]
		[HttpDelete("{productId}")]
		public IActionResult RemoveFavorite(string productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return BadRequest("Product ID is required.");
			}

			try
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User not authenticated.");
				}

				var removed = favoriteService.RemoveFavorite(userId, productId);

				if (!removed)
				{
					return NotFound("Favorite not found.");
				}

				return Json(new { message = "Product removed from favorites." });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize]
		[HttpPost("{productId}")]
		public IActionResult ToggleFavorite(string productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return BadRequest("Product ID is required.");
			}

			try
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User not authenticated.");
				}

				var isNowFavorite = favoriteService.ToggleFavorite(userId, productId);

				return Json(new
				{
					message = isNowFavorite ? "Product added to favorites." : "Product removed from favorites.",
					isFavorite = isNowFavorite
				});
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize]
		[HttpGet("{productId}")]
		public IActionResult IsFavorite(string productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return BadRequest("Product ID is required.");
			}

			try
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User not authenticated.");
				}

				var isFavorite = favoriteService.IsFavorite(userId, productId);

				return Json(new { isFavorite });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize]
		[HttpGet]
		public IActionResult GetFavoritesCount()
		{
			try
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User not authenticated.");
				}

				var count = favoriteService.GetFavoritesCount(userId);

				return Json(new { count });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
