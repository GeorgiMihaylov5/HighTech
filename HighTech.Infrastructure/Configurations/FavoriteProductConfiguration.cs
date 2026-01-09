using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainFavoriteProduct = HighTech.Core.Entities.FavoriteProduct;

namespace HighTech.Infrastructure.Configurations
{
	public class FavoriteProductConfiguration : IEntityTypeConfiguration<DomainFavoriteProduct>
	{
		public void Configure(EntityTypeBuilder<DomainFavoriteProduct> builder)
		{
			builder.HasKey(f => f.Id);
			builder.Property(f => f.Id).ValueGeneratedOnAdd();

			builder.Property(f => f.UserId).IsRequired();
			builder.Property(f => f.ProductId).IsRequired();
			builder.Property(f => f.CreatedAt).IsRequired();

			// Create a unique index to prevent duplicate favorites
			builder.HasIndex(f => new { f.UserId, f.ProductId }).IsUnique();

			builder.HasOne(f => f.User)
				.WithMany()
				.HasForeignKey(f => f.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(f => f.Product)
				.WithMany()
				.HasForeignKey(f => f.ProductId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
