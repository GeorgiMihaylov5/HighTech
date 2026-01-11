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
			builder.HasIndex(c => c.Name)
				.IsUnique()
				.HasFilter("[IsRemoved] = 0");

            builder.HasMany(c => c.Products)
				.WithOne(p => p.Category)
				.HasForeignKey(p => p.CategoryID)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(c => c.CategoryFields)
				.WithOne(cf => cf.Category)
				.HasForeignKey(cf => cf.CategoryId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
