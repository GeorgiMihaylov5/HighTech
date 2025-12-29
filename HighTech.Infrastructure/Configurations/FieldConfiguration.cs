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
			builder.HasIndex(f => f.Name).IsUnique();

			builder.HasMany(f => f.Categories)
				.WithOne(c => c.Field)
				.HasForeignKey(c => c.FieldId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
