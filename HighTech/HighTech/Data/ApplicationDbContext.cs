using HighTech.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HighTech.Data
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
        public DbSet<ProductCategory> ProductsCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>()
                .HasIndex(c => new { c.Id, c.FieldId }).IsUnique();

            builder.Entity<Field>()
                .HasMany(c => c.Categories)
                .WithOne(cf => cf.Field)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductCategory>()
                .HasOne(pf => pf.Category)
                .WithMany(c => c.ProductFields)
                .HasForeignKey(pf => pf.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<ProductField>()
            //   .HasOne(pf => pf.Category)
            //    .WithMany(c => c.ProductFields)
            //    .HasForeignKey(pf => new { pf.CategoryId, pf.FieldId });

            base.OnModelCreating(builder);
        }
    }
}