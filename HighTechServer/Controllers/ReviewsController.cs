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
    [Route("[controller]/[action]")]
    public class ReviewsController : Controller
    {
        private readonly IReviewService reviewService;

        public ReviewsController(IReviewService _reviewService)
        {
            reviewService = _reviewService;
        }

        public IActionResult GetByProduct(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return Response.ResultFailure<List<ReviewDTO>>("Product id cannot be null.", 400);
            }

            return Response.ResultSuccess(reviewService.GetByProduct(productId)
                .Select(ConvertToReviewDTO)
                .ToList());
        }

        [Authorize]
        public IActionResult GetMine(string productId, string username)
        {
            if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(username))
            {
                return Response.ResultFailure<ReviewDTO>("Product id and username are required.", 400);
            }

            if (!IsCurrentUser(username))
            {
                return Response.ResultFailure<ReviewDTO>("Cannot access another user's review.", 401);
            }

            var review = reviewService.GetByProductAndUsername(productId, username);

            if (review is null)
            {
                return Response.ResultSuccess<ReviewDTO>(null);
            }

            return Response.ResultSuccess(ConvertToReviewDTO(review));
        }

        [Authorize]
        [HttpPost]
        public IActionResult Upsert(UpsertReviewDTO dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.ProductId) || string.IsNullOrWhiteSpace(dto.Username))
            {
                return Response.ResultFailure<ReviewDTO>("Review data is required.", 400);
            }

            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return Response.ResultFailure<ReviewDTO>("Rating must be between 1 and 5.", 400);
            }

            if (dto.Comment is not null && dto.Comment.Length > StringLimits.LongText)
            {
                return Response.ResultFailure<ReviewDTO>($"Comment cannot be longer than {StringLimits.LongText} characters.", 400);
            }

            if (!IsCurrentUser(dto.Username))
            {
                return Response.ResultFailure<ReviewDTO>("Cannot save another user's review.", 401);
            }

            try
            {
                var review = reviewService.Upsert(dto.ProductId, dto.Username, dto.Rating, dto.Comment);

                if (review is null)
                {
                    return Response.ResultFailure<ReviewDTO>("Review cannot be saved.", 400);
                }

                return Response.ResultSuccess(ConvertToReviewDTO(review));
            }
            catch
            {
                return Response.ResultFailure<ReviewDTO>("An unexpected error occurred.", 500);
            }
        }

        [Authorize]
        [HttpDelete]
        public IActionResult Delete(string productId, string username)
        {
            if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(username))
            {
                return Response.ResultFailure<bool>("Product id and username are required.", 400);
            }

            if (!IsCurrentUser(username))
            {
                return Response.ResultFailure<bool>("Cannot remove another user's review.", 401);
            }

            try
            {
                var removed = reviewService.Remove(productId, username);

                if (!removed)
                {
                    return Response.ResultFailure<bool>("Review not found.", 404);
                }

                return Response.ResultSuccess(true);
            }
            catch
            {
                return Response.ResultFailure<bool>("An unexpected error occurred.", 500);
            }
        }

        private ReviewDTO ConvertToReviewDTO(Review review)
        {
            return new ReviewDTO()
            {
                Id = review.Id,
                ProductId = review.ProductId,
                UserId = review.UserId,
                Username = review.User?.UserName,
                FirstName = review.User?.FirstName,
                LastName = review.User?.LastName,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedOn = review.CreatedOn,
                UpdatedOn = review.UpdatedOn
            };
        }

        private bool IsCurrentUser(string username)
        {
            var currentUsername = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return currentUsername == username;
        }
    }
}
