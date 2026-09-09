using HighTech.Models;
using HighTech.Common;
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
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryField> CategoryFields { get; set; }
        public DbSet<ProductFieldValue> ProductFieldValues { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(StringLimits.Name);
                entity.Property(u => u.LastName).HasMaxLength(StringLimits.Name);
                entity.Property(u => u.PhoneNumber).HasMaxLength(StringLimits.Phone);
            });

            builder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Id).HasMaxLength(StringLimits.Guid);
                entity.Property(c => c.Name).HasMaxLength(StringLimits.CategoryName);
            });

            builder.Entity<CategoryField>(entity =>
            {
                entity.Property(cf => cf.Id).HasMaxLength(StringLimits.Guid);
                entity.Property(cf => cf.CategoryId).HasMaxLength(StringLimits.Guid);
                entity.Property(cf => cf.FieldId).HasMaxLength(StringLimits.Guid);
            });

            builder.Entity<Client>(entity =>
            {
                entity.Property(c => c.Id).HasMaxLength(StringLimits.Guid);
                entity.Property(c => c.Address).IsRequired().HasMaxLength(StringLimits.Address);
            });

            builder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(StringLimits.Guid);
                entity.Property(e => e.JobTitle).IsRequired().HasMaxLength(StringLimits.JobTitle);
            });

            builder.Entity<Favorite>(entity =>
            {
                entity.Property(f => f.Id).HasMaxLength(StringLimits.Guid);
                entity.Property(f => f.ProductId).HasMaxLength(StringLimits.Guid);
            });

            builder.Entity<Field>(entity =>
            {
                entity.Property(f => f.Id).HasMaxLength(StringLimits.Guid);
                entity.Property(f => f.Name).HasMaxLength(StringLimits.FieldName);
            });

            builder.Entity<Order>(entity =>
            {
                entity.Property(o => o.Id).HasMaxLength(StringLimits.Guid);
                entity.Property(o => o.Notes).HasMaxLength(StringLimits.LongText);
                entity.Property(o => o.City).HasMaxLength(StringLimits.City);
                entity.Property(o => o.PostalCode).HasMaxLength(StringLimits.PostalCode);
                entity.Property(o => o.DeliveryAddress).HasMaxLength(StringLimits.Address);
                entity.Property(o => o.PhoneNumber).HasMaxLength(StringLimits.Phone);
            });

            builder.Entity<OrderedProduct>(entity =>
            {
                entity.Property(op => op.Id).HasMaxLength(StringLimits.Guid);
                entity.Property(op => op.ProductId).HasMaxLength(StringLimits.Guid);
                entity.Property(op => op.OrderId).HasMaxLength(StringLimits.Guid);
                entity.Property(op => op.BuildId).HasMaxLength(StringLimits.Guid);
                entity.Property(op => op.OrderedPrice).HasPrecision(18, 2);
                entity.HasIndex(op => op.BuildId);
            });

            builder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Id).HasMaxLength(StringLimits.Guid);
                entity.Property(p => p.Manufacturer).HasMaxLength(StringLimits.ProductManufacturer);
                entity.Property(p => p.Model).HasMaxLength(StringLimits.ProductModel);
                entity.Property(p => p.Price).HasPrecision(18, 2);
                entity.Property(p => p.Discount).HasPrecision(18, 2);
                entity.Property(p => p.Image).HasMaxLength(StringLimits.ProductImage);
            });

            builder.Entity<ProductFieldValue>(entity =>
            {
                entity.ToTable("ProductFieldValues");
                entity.Property(pc => pc.Id).HasMaxLength(StringLimits.Guid);
                entity.Property(pc => pc.ProductId).HasMaxLength(StringLimits.Guid);
                entity.Property(pc => pc.CategoryFieldId).HasMaxLength(StringLimits.Guid);
                entity.Property(pc => pc.Value).HasMaxLength(StringLimits.ProductFieldValue);
            });

            builder.Entity<Review>(entity =>
            {
                entity.Property(r => r.Id).HasMaxLength(StringLimits.Guid);
                entity.Property(r => r.ProductId).HasMaxLength(StringLimits.Guid);
                entity.Property(r => r.Comment).HasMaxLength(StringLimits.LongText);
            });

            builder.Entity<CategoryField>()
                .HasIndex(cf => new { cf.CategoryId, cf.FieldId }).IsUnique();

            builder.Entity<Review>()
                .HasIndex(r => new { r.ProductId, r.UserId }).IsUnique();

            builder.Entity<Favorite>()
                .HasIndex(f => new { f.ProductId, f.UserId }).IsUnique();

            builder.Entity<Field>()
                .HasMany(f => f.CategoryFields)
                .WithOne(cf => cf.Field)
                .HasForeignKey(cf => cf.FieldId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CategoryField>()
                .HasOne(cf => cf.Category)
                .WithMany(c => c.CategoryFields)
                .HasForeignKey(cf => cf.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductFieldValue>()
                .HasOne(pf => pf.CategoryField)
                .WithMany(cf => cf.ProductFields)
                .HasForeignKey(pf => pf.CategoryFieldId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Favorite>()
                .HasOne(f => f.Product)
                .WithMany()
                .HasForeignKey(f => f.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }
    }
}
