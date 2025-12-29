using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using HighTech.Core.Services.Abstraction;
using HighTech.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HighTech.Infrastructure
{
	public static class ApplicationBuilderExtension
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? connectionString)
		{
			if(string.IsNullOrEmpty(connectionString))
			{
				throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }

            services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

			services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

            services.AddTransient<ICategoryRepository, CategoryRepository>();
			services.AddTransient<IClientRepository, ClientRepository>();
			services.AddTransient<IEmployeeRepository, EmployeeRepository>();
			services.AddTransient<IFieldRepository, FieldRepository>();
			services.AddTransient<IOrderRepository, OrderRepository>();
			services.AddTransient<IProductRepository, ProductRepository>();

			return services;
		}

		public static async Task<IApplicationBuilder> PrepareDatabase(this IApplicationBuilder app)
		{
			using var serviceScope = app.ApplicationServices.CreateScope();
			var services = serviceScope.ServiceProvider;

			await RoleSeeder(services);
			await SeedAdministrator(services);


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


		private static async Task SeedAdministrator(IServiceProvider serviceProvider)
		{
			var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
			var employeeService = serviceProvider.GetService<IEmployeeService>();

			if (employeeService is null)
			{
				throw new NullReferenceException("EmployeeService is null");
			}

			if (await userManager.FindByNameAsync("admin") is null)
			{
				var user = new AppUser
				{
					UserName = "admin",
					Email = "admin@admin.com",
					FirstName = "Admin",
					LastName = "Admin"
				};


				var result = await userManager.CreateAsync
				(user, "123!@#qweQWE");

				if (result.Succeeded)
				{
					userManager.AddToRoleAsync(user, "Administrator").Wait();
				}

				employeeService.CreateEmployee("Owner", user.Id);
			}
		}
	}
}