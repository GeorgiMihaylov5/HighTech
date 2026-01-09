using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using HighTech.Core.Services.Abstraction;

namespace HighTech.Core.Services
{
	public class ReviewService : IReviewService
	{
		private readonly IReviewRepository reviewRepository;

		public ReviewService(IReviewRepository _reviewRepository)
		{
			reviewRepository = _reviewRepository;
		}

		public ICollection<Review> GetProductReviews(string? productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				throw new ArgumentException("Product ID is required.", nameof(productId));
			}

			return reviewRepository.GetProductReviews(productId);
		}

		public ICollection<Review> GetUserReviews(string? userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentException("User ID is required.", nameof(userId));
			}

			return reviewRepository.GetUserReviews(userId);
		}

		public Review? GetReview(string? reviewId)
		{
			if (string.IsNullOrWhiteSpace(reviewId))
			{
				throw new ArgumentException("Review ID is required.", nameof(reviewId));
			}

			return reviewRepository.GetReview(reviewId);
		}

		public Review CreateReview(string? productId, int rating, string? comment, bool isAnonymous, string? userId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				throw new ArgumentException("Product ID is required.", nameof(productId));
			}

			if (rating < 1 || rating > 5)
			{
				throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));
			}

			if (!isAnonymous && string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentException("User ID is required for non-anonymous reviews.", nameof(userId));
			}

			// Trim and validate comment
			if (!string.IsNullOrWhiteSpace(comment))
			{
				comment = comment.Trim();
				if (comment.Length > 2000)
				{
					throw new ArgumentException("Comment cannot exceed 2000 characters.", nameof(comment));
				}
			}

			return reviewRepository.CreateReview(productId, rating, comment, isAnonymous, userId);
		}

		public Review? UpdateReview(string? reviewId, int rating, string? comment, string? userId)
		{
			if (string.IsNullOrWhiteSpace(reviewId))
			{
				throw new ArgumentException("Review ID is required.", nameof(reviewId));
			}

			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentException("User ID is required.", nameof(userId));
			}

			if (rating < 1 || rating > 5)
			{
				throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));
			}

			// Trim and validate comment
			if (!string.IsNullOrWhiteSpace(comment))
			{
				comment = comment.Trim();
				if (comment.Length > 2000)
				{
					throw new ArgumentException("Comment cannot exceed 2000 characters.", nameof(comment));
				}
			}

			return reviewRepository.UpdateReview(reviewId, rating, comment, userId);
		}

		public bool DeleteReview(string? reviewId, string? userId)
		{
			if (string.IsNullOrWhiteSpace(reviewId))
			{
				throw new ArgumentException("Review ID is required.", nameof(reviewId));
			}

			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentException("User ID is required.", nameof(userId));
			}

			return reviewRepository.DeleteReview(reviewId, userId);
		}

		public double GetAverageRating(string? productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return 0;
			}

			return reviewRepository.GetAverageRating(productId);
		}

		public int GetReviewCount(string? productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return 0;
			}

			return reviewRepository.GetReviewCount(productId);
		}

		public Dictionary<int, int> GetRatingDistribution(string? productId)
		{
			if (string.IsNullOrWhiteSpace(productId))
			{
				return new Dictionary<int, int>
				{
					{ 1, 0 },
					{ 2, 0 },
					{ 3, 0 },
					{ 4, 0 },
					{ 5, 0 }
				};
			}

			return reviewRepository.GetRatingDistribution(productId);
		}
	}
}
