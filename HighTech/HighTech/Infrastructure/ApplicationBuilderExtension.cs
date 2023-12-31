﻿using HighTech.Abstraction;
using HighTech.Models;
using Microsoft.AspNetCore.Identity;

namespace PernikComputers.Infrastructure
{
    public static class ApplicationBuilderExtension
    {
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