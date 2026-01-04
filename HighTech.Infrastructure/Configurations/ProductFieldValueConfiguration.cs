using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainProductFieldValue = HighTech.Core.Entities.ProductFieldValue;

namespace HighTech.Infrastructure.Configurations
{
	public class ProductFieldValueConfiguration : IEntityTypeConfiguration<DomainProductFieldValue>
	{
		public void Configure(EntityTypeBuilder<DomainProductFieldValue> builder)
		{
			builder.HasKey(pf => new { pf.ProductID, pf.FieldID });

			builder.Property(pf => pf.ProductID).IsRequired();
			builder.Property(pf => pf.FieldID).IsRequired();
			builder.Property(pf => pf.Value);

			builder.HasOne(pf => pf.Product)
				.WithMany(p => p.ProductFieldValues)
				.HasForeignKey(pf => pf.ProductID)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(pf => pf.Field)
				.WithMany(f => f.ProductFieldValues)
				.HasForeignKey(pf => pf.FieldID)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
