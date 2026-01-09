using HighTech.Core.Entities;
using HighTech.DTOs;

namespace HighTech.Mappers
{
	public static class ReviewMapper
	{
		public static ReviewDTO ToDTO(Review review)
		{
			if (review is null)
			{
				return null!;
			}

			return new ReviewDTO()
			{
				Id = review.Id,
				Rating = review.Rating,
				Comment = review.Comment,
				IsAnonymous = review.IsAnonymous,
				UserId = review.UserId,
				Username = review.IsAnonymous ? null : review.User?.UserName,
				UserFullName = review.IsAnonymous ? "Anonymous" : $"{review.User?.FirstName} {review.User?.LastName}",
				ProductId = review.ProductId,
				ProductName = review.Product != null ? $"{review.Product.Manufacturer} {review.Product.Model}" : null,
				CreatedAt = review.CreatedAt,
				UpdatedAt = review.UpdatedAt
			};
		}

		public static List<ReviewDTO> ToDTOList(ICollection<Review> reviews)
		{
			return reviews?.Select(r => ToDTO(r)).ToList() ?? new List<ReviewDTO>();
		}
	}
}
