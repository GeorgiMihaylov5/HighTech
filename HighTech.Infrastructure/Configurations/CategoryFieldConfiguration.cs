using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainCategoryField = HighTech.Core.Entities.CategoryField;

namespace HighTech.Infrastructure.Configurations
{
	public class CategoryFieldConfiguration : IEntityTypeConfiguration<DomainCategoryField>
	{
		public void Configure(EntityTypeBuilder<DomainCategoryField> builder)
		{
			builder.HasKey(cf => new { cf.CategoryId, cf.FieldId });

			builder.Property(cf => cf.CategoryId).IsRequired();
			builder.Property(cf => cf.FieldId).IsRequired();

			builder.HasOne(cf => cf.Category)
				.WithMany(c => c.CategoryFields)
				.HasForeignKey(cf => cf.CategoryId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(cf => cf.Field)
				.WithMany(f => f.CategoryFields)
				.HasForeignKey(cf => cf.FieldId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
