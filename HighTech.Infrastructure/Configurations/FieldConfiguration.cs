using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainField = HighTech.Core.Entities.Field;

namespace HighTech.Infrastructure.Configurations
{
	public class FieldConfiguration : IEntityTypeConfiguration<DomainField>
	{
		public void Configure(EntityTypeBuilder<DomainField> builder)
		{
			builder.HasKey(f => f.Id);
			builder.Property(f => f.Id).ValueGeneratedOnAdd();

			builder.Property(f => f.Name).IsRequired();
			builder.Property(f => f.TypeCode).IsRequired();
			builder.HasIndex(f => f.Name).IsUnique();

			builder.HasMany(f => f.CategoryFields)
				.WithOne(cf => cf.Field)
				.HasForeignKey(cf => cf.FieldId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(f => f.ProductFieldValues)
				.WithOne(pf => pf.Field)
				.HasForeignKey(pf => pf.FieldID)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
