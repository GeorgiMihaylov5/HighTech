using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainClient = HighTech.Core.Entities.Client;

namespace HighTech.Infrastructure.Configurations
{
	public class ClientConfiguration : IEntityTypeConfiguration<DomainClient>
	{
		public void Configure(EntityTypeBuilder<DomainClient> builder)
		{
			builder.HasKey(c => c.Id);
			builder.Property(c => c.Id).ValueGeneratedOnAdd();

			builder.Property(c => c.Address).IsRequired().HasMaxLength(40);
			builder.Property(c => c.UserId).IsRequired();

			// Configure the relationship to AppUser
			builder.HasOne(c => c.User)
				.WithMany()
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
