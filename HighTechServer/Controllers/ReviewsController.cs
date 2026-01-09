using HighTech.Core.Services.Abstraction;
using HighTech.DTOs;
using HighTech.Mappers;
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
				return BadRequest("Product ID is required.");
			}

			try
			{
				var reviews = reviewService.GetProductReviews(productId);
				return Json(ReviewMapper.ToDTOList(reviews));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize]
		[HttpGet]
		public IActionResult GetUserReviews()
		{
			try
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User not authenticated.");
				}

				var reviews = reviewService.GetUserReviews(userId);
				return Json(ReviewMapper.ToDTOList(reviews));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{reviewId}")]
		public IActionResult GetReview(string reviewId)
		{
			if (string.IsNullOrWhiteSpace(reviewId))
			{
				return BadRequest("Review ID is required.");
			}

			try
			{
				var review = reviewService.GetReview(reviewId);

				if (review == null)
				{
					return NotFound("Review not found.");
				}

				return Json(ReviewMapper.ToDTO(review));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		public IActionResult CreateReview([FromBody] CreateReviewRequest request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.ProductId))
			{
				return BadRequest("Product ID is required.");
			}

			if (request.Rating < 1 || request.Rating > 5)
			{
				return BadRequest("Rating must be between 1 and 5.");
			}

			try
			{
				string? userId = null;
				{
					userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
					if (string.IsNullOrEmpty(userId))
					{
						return Unauthorized("User must be authenticated for non-anonymous reviews.");
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
					return NotFound("Product not found or is no longer available.");
				}

				return Json(new { message = "Review created successfully.", review = ReviewMapper.ToDTO(review) });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize]
		[HttpPut("{reviewId}")]
		public IActionResult UpdateReview(string reviewId, [FromBody] UpdateReviewRequest request)
		{
			if (string.IsNullOrWhiteSpace(reviewId))
			{
				return BadRequest("Review ID is required.");
			}

			if (request == null || request.Rating < 1 || request.Rating > 5)
			{
				return BadRequest("Rating must be between 1 and 5.");
			}

			try
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User not authenticated.");
				}

				var review = reviewService.UpdateReview(reviewId, request.Rating, request.Comment, userId);

				if (review == null)
				{
					return NotFound("Review not found or you don't have permission to update it.");
				}

				return Json(new { message = "Review updated successfully.", review = ReviewMapper.ToDTO(review) });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize]
		[HttpDelete("{reviewId}")]
		public IActionResult DeleteReview(string reviewId)
		{
			if (string.IsNullOrWhiteSpace(reviewId))
			{
				return BadRequest("Review ID is required.");
			}

			try
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User not authenticated.");
				}

				var deleted = reviewService.DeleteReview(reviewId, userId);

				if (!deleted)
				{
					return NotFound("Review not found or you don't have permission to delete it.");
				}

				return Json(new { message = "Review deleted successfully." });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{productId}")]
		public IActionResult GetAverageRating(string productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return BadRequest("Product ID is required.");
			}

			try
			{
				var averageRating = reviewService.GetAverageRating(productId);
				var reviewCount = reviewService.GetReviewCount(productId);

				return Json(new
				{
					averageRating = Math.Round(averageRating, 2),
					reviewCount
				});
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{productId}")]
		public IActionResult GetRatingDistribution(string productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return BadRequest("Product ID is required.");
			}

			try
			{
				var distribution = reviewService.GetRatingDistribution(productId);
				return Json(distribution);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
