using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext context;

        public ReviewService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public ICollection<Review> GetByProduct(string productId)
        {
            return context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.UpdatedOn)
                .ToList();
        }

        public Review GetByProductAndUsername(string productId, string username)
        {
            return context.Reviews
                .Include(r => r.User)
                .FirstOrDefault(r => r.ProductId == productId && r.User.UserName == username);
        }

        public Review Upsert(string productId, string username, int rating, string comment)
        {
            if (rating < 1 || rating > 5)
            {
                return null;
            }

            var product = context.Products.FirstOrDefault(p => p.Id == productId && p.IsRemoved != true);
            var user = context.Users.FirstOrDefault(u => u.UserName == username);

            if (product is null || user is null)
            {
                return null;
            }

            var review = context.Reviews
                .FirstOrDefault(r => r.ProductId == productId && r.UserId == user.Id);

            if (review is null)
            {
                review = new Review()
                {
                    ProductId = productId,
                    UserId = user.Id,
                    Rating = rating,
                    Comment = comment,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                };

                context.Reviews.Add(review);
            }
            else
            {
                review.Rating = rating;
                review.Comment = comment;
                review.UpdatedOn = DateTime.UtcNow;
                context.Reviews.Update(review);
            }

            context.SaveChanges();

            return GetByProductAndUsername(productId, username);
        }

        public bool Remove(string productId, string username)
        {
            var review = context.Reviews
                .Include(r => r.User)
                .FirstOrDefault(r => r.ProductId == productId && r.User.UserName == username);

            if (review is null)
            {
                return false;
            }

            context.Reviews.Remove(review);
            return context.SaveChanges() != 0;
        }

        public decimal GetAverageRating(string productId)
        {
            var ratings = context.Reviews
                .Where(r => r.ProductId == productId)
                .Select(r => r.Rating)
                .ToList();

            if (ratings.Count == 0)
            {
                return 0;
            }

            return Math.Round((decimal)ratings.Average(), 2);
        }

        public int GetReviewCount(string productId)
        {
            return context.Reviews.Count(r => r.ProductId == productId);
        }
    }
}
