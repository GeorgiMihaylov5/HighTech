using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainProduct = HighTech.Core.Entities.Product;

namespace HighTech.Infrastructure.Configurations
{
	public class ProductConfiguration : IEntityTypeConfiguration<DomainProduct>
	{
		public void Configure(EntityTypeBuilder<DomainProduct> builder)
		{
			builder.HasKey(p => p.Id);
			builder.Property(p => p.Id).ValueGeneratedOnAdd();

			builder.Property(p => p.Manufacturer).IsRequired();
			builder.Property(p => p.Model).IsRequired();
			builder.Property(p => p.Warranty).IsRequired();
			builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
			builder.Property(p => p.Discount).HasColumnType("decimal(18,2)");
			builder.Property(p => p.Quantity).IsRequired();
			builder.Property(p => p.IsRemoved).IsRequired();

			builder.HasMany(p => p.ProductFields)
				.WithOne(pf => pf.Product)
				.HasForeignKey(pf => pf.ProductId);
		}
	}
}
