using HighTech.Core.Entities;

namespace HighTech.Core.Repositories
{
	public interface IReviewRepository
	{
		ICollection<Review> GetProductReviews(string? productId);
		ICollection<Review> GetUserReviews(string? userId);
		Review? GetReview(string? reviewId);
		Review CreateReview(string? productId, int rating, string? comment, bool isAnonymous, string? userId);
		Review? UpdateReview(string? reviewId, int rating, string? comment, string? userId);
		bool DeleteReview(string? reviewId, string? userId);
		double GetAverageRating(string? productId);
		int GetReviewCount(string? productId);
		Dictionary<int, int> GetRatingDistribution(string? productId);
	}
}
