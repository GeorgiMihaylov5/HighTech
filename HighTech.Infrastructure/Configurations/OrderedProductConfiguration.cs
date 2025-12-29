using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainOrderedProduct = HighTech.Core.Entities.OrderedProduct;

namespace HighTech.Infrastructure.Configurations
{
	public class OrderedProductConfiguration : IEntityTypeConfiguration<DomainOrderedProduct>
	{
		public void Configure(EntityTypeBuilder<DomainOrderedProduct> builder)
		{
			builder.HasKey(op => op.Id);
			builder.Property(op => op.Id).ValueGeneratedOnAdd();

			builder.Property(op => op.ProductId).IsRequired();
			builder.Property(op => op.OrderId).IsRequired();
			builder.Property(op => op.OrderedPrice).IsRequired().HasColumnType("decimal(18,2)");
			builder.Property(op => op.Count).IsRequired();

			builder.HasOne(op => op.Product)
				.WithMany()
				.HasForeignKey(op => op.ProductId);

			builder.HasOne(op => op.Order)
				.WithMany(o => o.OrderedProducts)
				.HasForeignKey(op => op.OrderId);
		}
	}
}
