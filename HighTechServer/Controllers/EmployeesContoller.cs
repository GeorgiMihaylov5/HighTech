using HighTech.Abstraction;
using HighTech.Common;
using HighTech.DTOs;
using HighTech.Models;
using HighTech.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace HighTech.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class EmployeesController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IEmployeeService employeeService;
        private readonly IAdminDashboardService dashboardService;
        private readonly EmployeeOptions employeeOptions;

        public EmployeesController(UserManager<AppUser> _userManager,
            IEmployeeService _employeeService,
            IAdminDashboardService _dashboardService,
            IOptions<EmployeeOptions> _employeeOptions)
        {
            userManager = _userManager;
            employeeService = _employeeService;
            dashboardService = _dashboardService;
            employeeOptions = _employeeOptions.Value;
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult GetDashboard()
        {
            try
            {
                return Response.ResultSuccess(dashboardService.GetDashboard());
            }
            catch
            {
                return Response.ResultFailure<AdminDashboardDTO>("An unexpected error occurred.", 500);
            }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAll()
        {
            var employeesDTO = employeeService
                .GetEmployees()
                .Select(e => new EmployeeDTO()
                {
                    Id = e.Id,
                    JobTitle = e.JobTitle,
                    UserId = e.User.Id,
                    FirstName = e.User.FirstName,
                    LastName = e.User.LastName,
                    Email = e.User.Email,
                    PhoneNumber = e.User.PhoneNumber,
                    Username = e.User.UserName,
                })
                .OrderBy(x => x.FirstName)
                .ToList();

            var adminIds = (await userManager
                .GetUsersInRoleAsync("Administrator"))
                .Select(a => a.Id)
                .ToHashSet();

            foreach (var employee in employeesDTO)
            {
                if (adminIds.Contains(employee.UserId))
                {
                    employee.IsAdmin = true;
                }
            }

            return Response.ResultSuccess(employeesDTO);
        }

        [Authorize(Roles = "Administrator,Employee")]
        public IActionResult GetByUsername(string username)
        {
            if (username is null)
            {
                return Response.ResultFailure<EmployeeDTO>($"There is not a user with {username} username.", 400);
            }

            var employee = employeeService.GetEmployeeByUsername(username);

            if (employee is null)
            {
                return Response.ResultFailure<EmployeeDTO>("Employee not found.", 404);
            }

            return Response.ResultSuccess(new EmployeeDTO()
            {
                Id = employee.Id,
                UserId = employee.UserId,
                Username = employee.User.UserName,
                Email = employee.User.Email,
                FirstName = employee.User.FirstName,
                LastName = employee.User.LastName,
                JobTitle = employee.JobTitle,
                PhoneNumber = employee.User.PhoneNumber
            });
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateEmployee(EmployeeDTO dto)
        {
            if (dto is null)
            {
                return Response.ResultFailure<EmployeeDTO>("Employee cannot be null.", 400);
            }

            var validationErrors = ValidateEmployee(dto);

            if (validationErrors.Count > 0)
            {
                return Response.ResultFailure<EmployeeDTO>(string.Join(", ", validationErrors), 400);
            }

            var employee = await userManager.FindByNameAsync(dto.Username);

            if (employee is not null)
            {
                return Response.ResultFailure<EmployeeDTO>("Employee exists!", 400);
            }

            var user = new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Username
            };

            var defaultPassword = employeeOptions.DefaultPassword
                ?? throw new InvalidOperationException("Employee:DefaultPassword is not configured.");

            var passwordValidationErrors = await ValidateDefaultPasswordAsync(user, defaultPassword);

            if (passwordValidationErrors.Count > 0)
            {
                return Response.ResultFailure<EmployeeDTO>(
                    $"Employee default password does not satisfy the configured password policy: {string.Join(", ", passwordValidationErrors)}",
                    400);
            }

            var result = await userManager.CreateAsync(user, defaultPassword);

            if (!result.Succeeded)
            {
                return Response.ResultFailure<EmployeeDTO>(string.Join(", ", result.Errors.Select(e => e.Description)), 400);
            }

            try
            {
                var emp = employeeService.CreateEmployee(dto.JobTitle, user.Id);
                dto.Id = emp.Id;

                userManager.AddToRoleAsync(user, "Employee").Wait();
            }
            catch (Exception ex)
            {
                return Response.ResultFailure<EmployeeDTO>("An unexpected error occurred.", 500);
            }

            return Response.ResultSuccess(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Promote(EmployeeDTO dto)
        {
            if (dto is null)
            {
                return Response.ResultFailure<bool>("User cannot be null!", 400);
            }

            var user = await userManager.FindByIdAsync(dto.UserId);

            if (user == null)
            {
                return Response.ResultFailure<bool>($"Cannot find user with {dto.UserId} id!", 404);
            }

            if(await userManager.IsInRoleAsync(user, "Administrator"))
            {
                return Response.ResultFailure<bool>("User is already in administrator role!", 400);
            }

            await userManager.AddToRoleAsync(user, "Administrator");

            return Response.ResultSuccess(true);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Demote(EmployeeDTO dto)
        {
            if (dto == null)
            {
                return Response.ResultFailure<bool>("User cannot be null!", 400);
            }

            var user = await userManager.FindByIdAsync(dto.UserId);

            if (user == null)
            {
                return Response.ResultFailure<bool>($"Cannot find user with {dto.UserId} id!", 404);
            }

            var currentUsername = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUsername == user.UserName)
            {
                return Response.ResultFailure<bool>("You cannot demote yourself.", 400);
            }

            if (!await userManager.IsInRoleAsync(user, "Administrator"))
            {
                return Response.ResultFailure<bool>("User isn't an administrator!", 400);
            }

            await userManager.RemoveFromRoleAsync(user, "Administrator");

            return Response.ResultSuccess(true);
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpPut]
        public IActionResult EditEmployee(EmployeeDTO dto)
        {
            var updatedEmp = employeeService.Update(dto.Id, dto.FirstName, dto.LastName, dto.PhoneNumber);

            if (updatedEmp)
            {
                return Response.ResultSuccess(dto);
            }

            return Response.ResultFailure<EmployeeDTO>("Failed to update employee.", 400);
        }

        [Authorize]
        public async Task<IActionResult> CheckUserRole(AuthUser token)
        {
            if (token is null)
            {
                return Response.ResultFailure<bool>("Token is null", 400);
            }

            if (token.Role.Contains("Client"))
            {
                return Response.ResultSuccess(true);
            }

            var roles = await userManager.GetRolesAsync(employeeService.GetEmployeeByUsername(token.Nameid).User);

            if (roles.Count != token.Role.Count)
            {
                return Response.ResultSuccess(false);
            }

            foreach (var tokenRole in token.Role)
            {
                if (!roles.Contains(tokenRole))
                {
                    return Response.ResultSuccess(false);
                }
            }

            return Response.ResultSuccess(true);
        }

        private static List<string> ValidateEmployee(EmployeeDTO dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                errors.Add("First name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                errors.Add("Last name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.JobTitle))
            {
                errors.Add("Job title is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                errors.Add("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                errors.Add("Username is required.");
            }

            errors.AddRange(StringLimits.ValidateUserText(dto.FirstName, dto.LastName, dto.Email, dto.Username, dto.PhoneNumber));
            StringLimits.AddMaxLengthError(errors, "Job title", dto.JobTitle, StringLimits.JobTitle);

            return errors;
        }

        private async Task<List<string>> ValidateDefaultPasswordAsync(AppUser user, string password)
        {
            var errors = new List<string>();

            foreach (var passwordValidator in userManager.PasswordValidators)
            {
                var result = await passwordValidator.ValidateAsync(userManager, user, password);

                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors.Select(e => e.Description));
                }
            }

            return errors.Distinct().ToList();
        }
    }
}
