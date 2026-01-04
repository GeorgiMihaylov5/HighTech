using HighTech.Core.Entities;
using HighTech.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Infrastructure
{
	public class ApplicationDbContext : IdentityDbContext<AppUser>
	{
		public ApplicationDbContext(DbContextOptions options)
			: base(options)
		{

		}

		public DbSet<Employee> Employees { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<Field> Fields { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderedProduct> OrderedProducts { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<CategoryField> CategoryFields { get; set; }
		public DbSet<ProductFieldValue> ProductFieldValues { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.ApplyConfiguration(new CategoryConfiguration());
			builder.ApplyConfiguration(new ClientConfiguration());
			builder.ApplyConfiguration(new EmployeeConfiguration());
			builder.ApplyConfiguration(new FieldConfiguration());
			builder.ApplyConfiguration(new OrderConfiguration());
			builder.ApplyConfiguration(new OrderedProductConfiguration());
			builder.ApplyConfiguration(new ProductConfiguration());
			builder.ApplyConfiguration(new CategoryFieldConfiguration());
			builder.ApplyConfiguration(new ProductFieldValueConfiguration());
		}
	}
}