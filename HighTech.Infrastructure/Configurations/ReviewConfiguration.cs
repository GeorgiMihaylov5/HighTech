using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainReview = HighTech.Core.Entities.Review;

namespace HighTech.Infrastructure.Configurations
{
	public class ReviewConfiguration : IEntityTypeConfiguration<DomainReview>
	{
		public void Configure(EntityTypeBuilder<DomainReview> builder)
		{
			builder.HasKey(r => r.Id);
			builder.Property(r => r.Id).ValueGeneratedOnAdd();

			builder.Property(r => r.Rating).IsRequired();
			builder.Property(r => r.Comment).HasMaxLength(2000);
			builder.Property(r => r.IsAnonymous).IsRequired();
			builder.Property(r => r.CreatedAt).IsRequired();
			builder.Property(r => r.ProductId).IsRequired();

			// UserId is nullable for anonymous reviews
			builder.Property(r => r.UserId).IsRequired(false);

			// Indexes for better query performance
			builder.HasIndex(r => r.ProductId);
			builder.HasIndex(r => r.UserId);
			builder.HasIndex(r => r.CreatedAt);

			builder.HasOne(r => r.User)
				.WithMany()
				.HasForeignKey(r => r.UserId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.HasOne(r => r.Product)
				.WithMany()
				.HasForeignKey(r => r.ProductId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
