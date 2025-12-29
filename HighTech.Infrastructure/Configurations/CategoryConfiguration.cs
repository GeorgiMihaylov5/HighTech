using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainCategory = HighTech.Core.Entities.Category;

namespace HighTech.Infrastructure.Configurations
{
	public class CategoryConfiguration : IEntityTypeConfiguration<DomainCategory>
	{
		public void Configure(EntityTypeBuilder<DomainCategory> builder)
		{
			builder.HasKey(c => c.Id);
			builder.Property(c => c.Id).ValueGeneratedOnAdd();

			builder.Property(c => c.Name).IsRequired();
			builder.Property(c => c.FieldId).IsRequired();

			builder.HasIndex(c => new { c.Id, c.FieldId }).IsUnique();

			builder.HasOne(c => c.Field)
				.WithMany(f => f.Categories)
				.HasForeignKey(c => c.FieldId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
