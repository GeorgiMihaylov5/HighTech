using HighTech.Core.Services.Abstraction;
using HighTech.DTOs;
using HighTech.Mappers;
using HighTechServer;
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

		[HttpGet("{productId}")]
		public IActionResult GetProductReviews(string productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return Response.Error("Product ID is required.", ErrorCode.ReviewProductIdMissing, 400);
			}

			var reviews = reviewService.GetProductReviews(productId);
			return Response.Success(ReviewMapper.ToDTOList(reviews));
		}

		[Authorize]
		[HttpGet]
		public IActionResult GetUserReviews()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Response.Error("User not authenticated.", ErrorCode.UnauthorizedAccess, 401);
			}

			var reviews = reviewService.GetUserReviews(userId);
			return Response.Success(ReviewMapper.ToDTOList(reviews));
		}

		[HttpGet("{reviewId}")]
		public IActionResult GetReview(string reviewId)
		{
			if (string.IsNullOrWhiteSpace(reviewId))
			{
				return Response.Error("Review ID is required.", ErrorCode.ReviewIdMissing, 400);
			}

			var review = reviewService.GetReview(reviewId);

			if (review == null)
			{
				return Response.Error("Review not found.", ErrorCode.ReviewNotFound, 404);
			}

			return Response.Success(ReviewMapper.ToDTO(review));
		}

		[HttpPost]
		public IActionResult CreateReview([FromBody] CreateReviewRequest request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.ProductId))
			{
				return Response.Error("Product ID is required.", ErrorCode.ReviewProductIdMissing, 400);
			}

			if (request.Rating < 1 || request.Rating > 5)
			{
				return Response.Error("Rating must be between 1 and 5.", ErrorCode.ReviewRatingInvalid, 400);
			}

			string? userId = null;
			{
				userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
				{
					return Response.Error("User must be authenticated for non-anonymous reviews.", ErrorCode.UnauthorizedAccess, 401);
				}
			}

			var review = reviewService.CreateReview(
				request.ProductId,
				request.Rating,
				request.Comment,
				request.IsAnonymous,
				userId);

			if (review == null)
			{
				return Response.Error("Product not found or is no longer available.", ErrorCode.ProductNotFound, 404);
			}

			return Response.Success(new { message = "Review created successfully.", review = ReviewMapper.ToDTO(review) }, 201);
		}

		[Authorize]
		[HttpPut("{reviewId}")]
		public IActionResult UpdateReview(string reviewId, [FromBody] UpdateReviewRequest request)
		{
			if (string.IsNullOrWhiteSpace(reviewId))
			{
				return Response.Error("Review ID is required.", ErrorCode.ReviewIdMissing, 400);
			}

			if (request == null || request.Rating < 1 || request.Rating > 5)
			{
				return Response.Error("Rating must be between 1 and 5.", ErrorCode.ReviewRatingInvalid, 400);
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Response.Error("User not authenticated.", ErrorCode.UnauthorizedAccess, 401);
			}

			var review = reviewService.UpdateReview(reviewId, request.Rating, request.Comment, userId);

			if (review == null)
			{
				return Response.Error("Review not found or you don't have permission to update it.", ErrorCode.ReviewNotFound, 404);
			}

			return Response.Success(new { message = "Review updated successfully.", review = ReviewMapper.ToDTO(review) });
		}

		[Authorize]
		[HttpDelete("{reviewId}")]
		public IActionResult DeleteReview(string reviewId)
		{
			if (string.IsNullOrWhiteSpace(reviewId))
			{
				return Response.Error("Review ID is required.", ErrorCode.ReviewIdMissing, 400);
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Response.Error("User not authenticated.", ErrorCode.UnauthorizedAccess, 401);
			}

			var deleted = reviewService.DeleteReview(reviewId, userId);

			if (!deleted)
			{
				return Response.Error("Review not found or you don't have permission to delete it.", ErrorCode.ReviewNotFound, 404);
			}

			return Response.Success(new { message = "Review deleted successfully." });
		}

		[HttpGet("{productId}")]
		public IActionResult GetAverageRating(string productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return Response.Error("Product ID is required.", ErrorCode.ReviewProductIdMissing, 400);
			}

			var averageRating = reviewService.GetAverageRating(productId);
			var reviewCount = reviewService.GetReviewCount(productId);

			return Response.Success(new
			{
				averageRating = Math.Round(averageRating, 2),
				reviewCount
			});
		}

		[HttpGet("{productId}")]
		public IActionResult GetRatingDistribution(string productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return Response.Error("Product ID is required.", ErrorCode.ReviewProductIdMissing, 400);
			}

			var distribution = reviewService.GetRatingDistribution(productId);
			return Response.Success(distribution);
		}
	}
}
