using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainProductCategory = HighTech.Core.Entities.ProductCategory;

namespace HighTech.Infrastructure.Configurations
{
	public class ProductCategoryConfiguration : IEntityTypeConfiguration<DomainProductCategory>
	{
		public void Configure(EntityTypeBuilder<DomainProductCategory> builder)
		{
			builder.HasKey(pc => pc.Id);
			builder.Property(pc => pc.Id).ValueGeneratedOnAdd();

			builder.Property(pc => pc.ProductId).IsRequired();
			builder.Property(pc => pc.CategoryId).IsRequired();
			builder.Property(pc => pc.Value).IsRequired();

			builder.HasOne(pc => pc.Product)
				.WithMany(p => p.ProductFields)
				.HasForeignKey(pc => pc.ProductId);

            //TODO where we need a cascade delete?
            builder.HasOne(pc => pc.Category)
				.WithMany(c => c.ProductFields)
				.HasForeignKey(pc => pc.CategoryId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
