using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Infrastructure.Repositories
{
	public class ReviewRepository : IReviewRepository
	{
		private readonly ApplicationDbContext context;

		public ReviewRepository(ApplicationDbContext _context)
		{
			context = _context;
		}

		public ICollection<Review> GetProductReviews(string? productId)
		{
			return context.Reviews
				.Where(r => r.ProductId == productId)
				.Include(r => r.User)
				.OrderByDescending(r => r.CreatedAt)
				.ToList();
		}

		public ICollection<Review> GetUserReviews(string? userId)
		{
			return context.Reviews
				.Where(r => r.UserId == userId)
				.Include(r => r.Product)
				.OrderByDescending(r => r.CreatedAt)
				.ToList();
		}

		public Review? GetReview(string? reviewId)
		{
			return context.Reviews
				.Include(r => r.User)
				.Include(r => r.Product)
				.FirstOrDefault(r => r.Id == reviewId);
		}

		public Review CreateReview(string? productId, int rating, string? comment, bool isAnonymous, string? userId)
		{
			// Verify product exists
			var product = context.Products.Find(productId);
			if (product == null || product.IsRemoved)
			{
                //TODO add custom exception
                throw new ArgumentException("Product doesn't exist or it is removed!");
            }

            var user = context.Users.Find(userId);
			if (user == null)
			{
				throw new ArgumentException("User doesn't exist!");
			}

			var review = new Review
			{
				ProductId = productId,
				Rating = rating,
				Comment = comment,
				IsAnonymous = isAnonymous,
				UserId = isAnonymous ? null : userId,
				CreatedAt = DateTime.UtcNow
			};

			context.Reviews.Add(review);
			context.SaveChanges();

			review.Product = product;
			review.User = user;
			return review;	
		}

		public Review? UpdateReview(string? reviewId, int rating, string? comment, string? userId)
		{
			var review = context.Reviews.FirstOrDefault(r => r.Id == reviewId);

			if (review == null || review.UserId != userId)
			{
				return null;
			}

			review.Rating = rating;
			review.Comment = comment;
			review.UpdatedAt = DateTime.UtcNow;

			context.SaveChanges();

			return review;

        }

		public bool DeleteReview(string? reviewId, string? userId)
		{
			var review = context.Reviews.FirstOrDefault(r => r.Id == reviewId);

			if (review == null || review.UserId != userId)
			{
				return false;
			}

			context.Reviews.Remove(review);
			context.SaveChanges();

			return true;
		}

		public double GetAverageRating(string? productId)
		{
			var reviews = context.Reviews.Where(r => r.ProductId == productId);

			if (!reviews.Any())
			{
				return 0;
			}

			return reviews.Average(r => r.Rating);
		}

		public int GetReviewCount(string? productId)
		{
			return context.Reviews.Count(r => r.ProductId == productId);
		}

		public Dictionary<int, int> GetRatingDistribution(string? productId)
		{
			var distribution = new Dictionary<int, int>
			{
				{ 1, 0 },
				{ 2, 0 },
				{ 3, 0 },
				{ 4, 0 },
				{ 5, 0 }
			};

			var ratings = context.Reviews
				.Where(r => r.ProductId == productId)
				.GroupBy(r => r.Rating)
				.Select(g => new { Rating = g.Key, Count = g.Count() })
				.ToList();

			foreach (var rating in ratings)
			{
				if (rating.Rating >= 1 && rating.Rating <= 5)
				{
					distribution[rating.Rating] = rating.Count;
				}
			}

			return distribution;
		}
	}
}
