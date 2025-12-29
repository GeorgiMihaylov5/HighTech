using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainOrder = HighTech.Core.Entities.Order;

namespace HighTech.Infrastructure.Configurations
{
	public class OrderConfiguration : IEntityTypeConfiguration<DomainOrder>
	{
		public void Configure(EntityTypeBuilder<DomainOrder> builder)
		{
			builder.HasKey(o => o.Id);
			builder.Property(o => o.Id).ValueGeneratedOnAdd();

			builder.Property(o => o.OrderedOn).IsRequired();
			builder.Property(o => o.CustomerId).IsRequired();
			builder.Property(o => o.Status).IsRequired();

			// Ignore the navigation property since we're mapping to Infrastructure.Identity.AppUser
			builder.Ignore(o => o.Customer);

			builder.HasMany(o => o.OrderedProducts)
				.WithOne(op => op.Order)
				.HasForeignKey(op => op.OrderId);
		}
	}
}
