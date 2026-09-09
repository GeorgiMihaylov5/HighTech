using HighTech.Abstraction;
using HighTech.Configurator;
using HighTech.Data;
using HighTech.Models;
using HighTech.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Infrastructure
{
    public static class ApplicationBuilderExtension
    {
        public static async Task<IApplicationBuilder> PrepareDatabase(this IApplicationBuilder app, SeedingOptions seedingOptions)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var services = serviceScope.ServiceProvider;

            await RoleSeeder(services);
            await SeedAdministrator(services, seedingOptions);
            SeedPcConfiguratorMetadata(services);

            return app;
        }

        private static async Task RoleSeeder(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Administrator", "Client", "Employee" };

            IdentityResult roleResult;

            foreach (var role in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);

                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdministrator(IServiceProvider serviceProvider, SeedingOptions seedingOptions)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var employeeService = serviceProvider.GetService<IEmployeeService>()
                ?? throw new NullReferenceException("EmployeeService is null");

            if (await userManager.FindByNameAsync("admin") is null)
            {
                var user = new AppUser
                {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    FirstName = "Admin",
                    LastName = "Admin"
                };

                var adminPassword = seedingOptions.AdminPassword
                    ?? throw new InvalidOperationException("Seeding:AdminPassword is not configured.");

                var result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }

                employeeService.CreateEmployee("Owner", user.Id);
            }
        }

        private static void SeedPcConfiguratorMetadata(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // ---- Fields (unique by Name) ----
            var fieldDefs = new[]
            {
                (PcConfiguratorConstants.Fields.SocketType,                      TypeCode.String),
                (PcConfiguratorConstants.Fields.TdpW,                            TypeCode.Int32),
                (PcConfiguratorConstants.Fields.GraphicCore,                     TypeCode.String),
                (PcConfiguratorConstants.Fields.RamType,                         TypeCode.String),
                (PcConfiguratorConstants.Fields.MotherboardFormFactor,           TypeCode.String),
                (PcConfiguratorConstants.Fields.SupportedMotherboardFormFactors, TypeCode.String),
                (PcConfiguratorConstants.Fields.MaxRamSpeedMhz,                  TypeCode.Int32),
                (PcConfiguratorConstants.Fields.RamSpeedMhz,                     TypeCode.Int32),
                (PcConfiguratorConstants.Fields.LengthMm,                        TypeCode.Int32),
                (PcConfiguratorConstants.Fields.MaxGpuLengthMm,                  TypeCode.Int32),
                (PcConfiguratorConstants.Fields.CaseType,                        TypeCode.String),
                (PcConfiguratorConstants.Fields.CaseLengthMm,                    TypeCode.Int32),
                (PcConfiguratorConstants.Fields.CaseWidthMm,                     TypeCode.Int32),
                (PcConfiguratorConstants.Fields.CaseHeightMm,                    TypeCode.Int32),
                (PcConfiguratorConstants.Fields.RecommendedPsuW,                 TypeCode.Int32),
                (PcConfiguratorConstants.Fields.WattageW,                        TypeCode.Int32),
                (PcConfiguratorConstants.Fields.HeightMm,                        TypeCode.Int32),
                (PcConfiguratorConstants.Fields.MaxCpuCoolerHeightMm,            TypeCode.Int32),
                (PcConfiguratorConstants.Fields.StorageInterface,                TypeCode.String),
                (PcConfiguratorConstants.Fields.M2Slots,                         TypeCode.Int32),
                (PcConfiguratorConstants.Fields.SataPorts,                       TypeCode.Int32),
            };

            var existingFields = context.Fields
                .ToDictionary(f => f.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var (name, typeCode) in fieldDefs)
            {
                if (!existingFields.ContainsKey(name))
                {
                    var field = new Field { Name = name, TypeCode = typeCode };
                    context.Fields.Add(field);
                    context.SaveChanges();
                    existingFields[name] = field;
                }
            }

            existingFields = context.Fields
                .ToDictionary(f => f.Name, StringComparer.OrdinalIgnoreCase);

            var existingCategories = context.Categories
                .Include(c => c.CategoryFields)
                .ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var catName in PcConfiguratorConstants.Categories.All)
            {
                if (!existingCategories.ContainsKey(catName))
                {
                    var category = new Category { Name = catName, CategoryFields = new List<CategoryField>() };
                    context.Categories.Add(category);
                    context.SaveChanges(); 
                    existingCategories[catName] = category;
                }
            }

            existingCategories = context.Categories
                .Include(c => c.CategoryFields)
                .ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

            var existingPairings = context.CategoryFields.ToHashSet();

            foreach (var (catName, fieldName) in PcConfiguratorConstants.RequiredPairings)
            {
                if (!existingCategories.TryGetValue(catName, out var category))
                    continue;
                if (!existingFields.TryGetValue(fieldName, out var field))
                    continue;

                var alreadyExists = category.CategoryFields.Any(cf => cf.FieldId == field.Id);
                if (!alreadyExists)
                {
                    context.CategoryFields.Add(new CategoryField
                    {
                        CategoryId = category.Id,
                        FieldId = field.Id
                    });
                }
            }

            context.SaveChanges();

            if (existingCategories.TryGetValue(PcConfiguratorConstants.Categories.Assembly, out _))
            {
                var hasAssemblyProduct = context.Products
                    .Any(p => p.Manufacturer == PcConfiguratorConstants.AssemblyProduct.Manufacturer
                        && p.Model == PcConfiguratorConstants.AssemblyProduct.Model
                        && p.IsRemoved != true);

                if (!hasAssemblyProduct)
                {
                    var assemblyProduct = new Product
                    {
                        Manufacturer = PcConfiguratorConstants.AssemblyProduct.Manufacturer,
                        Model = PcConfiguratorConstants.AssemblyProduct.Model,
                        Price = 50.00m,
                        Discount = 0,
                        Quantity = 100000,
                        Warranty = 0,
                        Image = "",
                        IsRemoved = false
                    };
                    context.Products.Add(assemblyProduct);
                    context.SaveChanges();
                }
            }
        }
    }
}
