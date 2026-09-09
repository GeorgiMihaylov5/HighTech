using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IReviewService
    {
        public ICollection<Review> GetByProduct(string productId);
        public Review GetByProductAndUsername(string productId, string username);
        public Review Upsert(string productId, string username, int rating, string comment);
        public bool Remove(string productId, string username);
        public decimal GetAverageRating(string productId);
        public int GetReviewCount(string productId);
    }
}
