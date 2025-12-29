using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainEmployee = HighTech.Core.Entities.Employee;

namespace HighTech.Infrastructure.Configurations
{
	public class EmployeeConfiguration : IEntityTypeConfiguration<DomainEmployee>
	{
		public void Configure(EntityTypeBuilder<DomainEmployee> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			builder.Property(e => e.JobTitle).IsRequired().HasMaxLength(30);
			builder.Property(e => e.UserId).IsRequired();

			// Configure the relationship to AppUser
			builder.HasOne(e => e.User)
				.WithMany()
				.HasForeignKey(e => e.UserId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
