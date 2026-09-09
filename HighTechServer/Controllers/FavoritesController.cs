using HighTech.Abstraction;
using HighTech.Common;
using HighTech.DTOs;
using HighTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HighTech.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public class FavoritesController : Controller
    {
        private readonly IFavoriteService favoriteService;
        private readonly ICategoryService categoryService;
        private readonly IFieldService fieldService;
        private readonly IReviewService reviewService;

        public FavoritesController(IFavoriteService _favoriteService,
            ICategoryService _categoryService,
            IFieldService _fieldService,
            IReviewService _reviewService)
        {
            favoriteService = _favoriteService;
            categoryService = _categoryService;
            fieldService = _fieldService;
            reviewService = _reviewService;
        }

        public IActionResult GetMine(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return Response.ResultFailure<List<FavoriteDTO>>("Username is required.", 400);
            }

            if (!IsCurrentUser(username))
            {
                return Response.ResultFailure<List<FavoriteDTO>>("Cannot access another user's favorites.", 401);
            }

            var favorites = favoriteService.GetByUsername(username)
                .Select(ConvertToFavoriteDTO)
                .ToList();

            return Response.ResultSuccess(favorites);
        }

        public IActionResult GetCount(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return Response.ResultFailure<int>("Username is required.", 400);
            }

            if (!IsCurrentUser(username))
            {
                return Response.ResultFailure<int>("Cannot access another user's favorites.", 401);
            }

            return Response.ResultSuccess(favoriteService.GetCount(username));
        }

        public IActionResult IsFavorite(string productId, string username)
        {
            if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(username))
            {
                return Response.ResultFailure<bool>("Product id and username are required.", 400);
            }

            if (!IsCurrentUser(username))
            {
                return Response.ResultFailure<bool>("Cannot access another user's favorites.", 401);
            }

            return Response.ResultSuccess(favoriteService.IsFavorite(productId, username));
        }

        [HttpPost]
        public IActionResult Add(FavoriteActionDTO dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.ProductId) || string.IsNullOrWhiteSpace(dto.Username))
            {
                return Response.ResultFailure<FavoriteDTO>("Favorite data is required.", 400);
            }

            if (!IsCurrentUser(dto.Username))
            {
                return Response.ResultFailure<FavoriteDTO>("Cannot update another user's favorites.", 401);
            }

            try
            {
                var favorite = favoriteService.Add(dto.ProductId, dto.Username);

                if (favorite is null)
                {
                    return Response.ResultFailure<FavoriteDTO>("Favorite cannot be saved.", 400);
                }

                return Response.ResultSuccess(ConvertToFavoriteDTO(favorite));
            }
            catch
            {
                return Response.ResultFailure<FavoriteDTO>("An unexpected error occurred.", 500);
            }
        }

        [HttpDelete]
        public IActionResult Delete(string productId, string username)
        {
            if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(username))
            {
                return Response.ResultFailure<bool>("Product id and username are required.", 400);
            }

            if (!IsCurrentUser(username))
            {
                return Response.ResultFailure<bool>("Cannot update another user's favorites.", 401);
            }

            try
            {
                var removed = favoriteService.Remove(productId, username);

                if (!removed)
                {
                    return Response.ResultFailure<bool>("Favorite not found.", 404);
                }

                return Response.ResultSuccess(true);
            }
            catch
            {
                return Response.ResultFailure<bool>("An unexpected error occurred.", 500);
            }
        }

        [HttpDelete]
        public IActionResult Clear(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return Response.ResultFailure<bool>("Username is required.", 400);
            }

            if (!IsCurrentUser(username))
            {
                return Response.ResultFailure<bool>("Cannot update another user's favorites.", 401);
            }

            try
            {
                return Response.ResultSuccess(favoriteService.Clear(username));
            }
            catch
            {
                return Response.ResultFailure<bool>("An unexpected error occurred.", 500);
            }
        }

        private FavoriteDTO ConvertToFavoriteDTO(Favorite favorite)
        {
            return new FavoriteDTO()
            {
                Id = favorite.Id,
                ProductId = favorite.ProductId,
                UserId = favorite.UserId,
                CreatedOn = favorite.CreatedOn,
                Product = ConvertToProductDTO(favorite.Product)
            };
        }

        private ProductDTO ConvertToProductDTO(Product product)
        {
            if (product is null)
            {
                return null;
            }

            var categoryName = categoryService.GetCategoryByProduct(product.Id);
            var dto = new ProductDTO()
            {
                Id = product.Id,
                Manufacturer = product.Manufacturer,
                Model = product.Model,
                Price = product.Price,
                Warranty = product.Warranty,
                Discount = product.Discount,
                Image = product.Image,
                Quantity = product.Quantity,
                CategoryName = categoryName,
                Fields = new List<FieldDTO>(),
                AverageRating = reviewService.GetAverageRating(product.Id),
                ReviewCount = reviewService.GetReviewCount(product.Id),
                Reviews = new List<ReviewDTO>()
            };

            var productFields = fieldService.GetProductFields(product.Id);

            foreach (var pf in productFields)
            {
                dto.Fields.Add(new FieldDTO()
                {
                    Id = pf.CategoryField.FieldId,
                    Name = pf.CategoryField.Field.Name,
                    TypeCode = pf.CategoryField.Field.TypeCode,
                    Value = pf.Value
                });
            }

            return dto;
        }

        private bool IsCurrentUser(string username)
        {
            var currentUsername = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return currentUsername == username;
        }
    }
}
