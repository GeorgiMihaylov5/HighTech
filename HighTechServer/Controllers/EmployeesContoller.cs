using HighTech.Core.Entities;
using HighTech.Core.Services.Abstraction;
using HighTech.DTOs;
using HighTech.Mappers;
using HighTechServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
	[ApiController]
	[Route("[controller]/[action]")]
	public class EmployeesController : Controller
	{
		private readonly UserManager<AppUser> userManager;
		private readonly IEmployeeService employeeService;

		public EmployeesController(UserManager<AppUser> _userManager, IEmployeeService _employeeService)
		{
			userManager = _userManager;
			employeeService = _employeeService;
		}

		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> GetAll()
		{
			var employeesDTO = EmployeeMapper.ToDTOList(employeeService.GetEmployees())
				.OrderBy(x => x.FirstName)
				.ToList();

			var admins = (await userManager
				.GetUsersInRoleAsync("Administrator"))
				.OrderBy(x => x.FirstName)
				.ThenBy(x => x.Id)
				.ToList();

			for (int i = 0; i < Math.Min(employeesDTO.Count, admins.Count); i++)
			{
				if (employeesDTO[i].UserId == admins[i].Id)
				{
					employeesDTO[i].IsAdmin = true;
				}
			}

			return Response.Success(employeesDTO);
		}

		[Authorize(Roles = "Administrator,Employee")]
		public IActionResult GetByUsername(string username)
		{
			if (username is null)
			{
				return Response.Error($"There is not a user with {username} username.", ErrorCode.EmployeeUsernameMissing, 400);
			}

			var employee = employeeService.GetEmployeeByUsername(username);

			if (employee is null)
			{
				return Response.Success<EmployeeDTO?>(null);
			}

			return Response.Success(EmployeeMapper.ToDTO(employee));
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> CreateEmployee(EmployeeDTO dto)
		{
			if (dto is null || dto.Username is null)
			{
				return Response.Error("Employee cannot be null!", ErrorCode.InvalidRequest, 400);
			}

			var employee = await userManager.FindByNameAsync(dto.Username);

			if (employee is not null)
			{
				return Response.Error("Employee exists!", ErrorCode.EmployeeAlreadyExists, 400);
			}

			var user = new AppUser
			{
				FirstName = dto.FirstName,
				LastName = dto.LastName,
				Email = dto.Email,
				UserName = dto.Username
			};

			var result = await userManager.CreateAsync(user, "employee123");

			if (!result.Succeeded)
			{
				return Response.Error(string.Join(", ", result.Errors.Select(e => e.Description)), ErrorCode.EmployeeCreateError, 400);
			}

			try
			{
				var emp = employeeService.CreateEmployee(dto.JobTitle, user.Id);
				dto.Id = emp.Id;

				userManager.AddToRoleAsync(user, "Employee").Wait();
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.EmployeeCreateError, 400);
			}

			return Response.Success(dto, 201);
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Promote(EmployeeDTO dto)
		{
			if (dto is null || dto.UserId is null)
			{
				return Response.Error("User cannot be null!", ErrorCode.InvalidRequest, 400);
			}

			var user = await userManager.FindByIdAsync(dto.UserId);

			if (user == null)
			{
				return Response.Error($"Cannot find user with {dto.UserId} id!", ErrorCode.EmployeeNotFound, 404);
			}

			if (await userManager.IsInRoleAsync(user, "Administrator"))
			{
				return Response.Error("User is already in admininstrator role!", ErrorCode.EmployeePromotionError, 400);
			}

			await userManager.AddToRoleAsync(user, "Administrator");

			return Ok();
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Demote(EmployeeDTO dto)
		{
			if (dto == null || dto.UserId == null)
			{
				return Response.Error("User cannot be null!", ErrorCode.InvalidRequest, 400);
			}
			var user = await userManager.FindByIdAsync(dto.UserId);

			if (user == null)
			{
				return Response.Error($"Cannot find user with {dto.UserId} id!", ErrorCode.EmployeeNotFound, 404);
			}

			if (!await userManager.IsInRoleAsync(user, "Administrator"))
			{
				return Response.Error("User isn't an administaror!", ErrorCode.EmployeeDemotionError, 400);
			}

			await userManager.RemoveFromRoleAsync(user, "Administrator");

			return Ok();
		}

		[Authorize(Roles = "Administrator,Employee")]
		[HttpPut]
		public IActionResult EditEmployee(EmployeeDTO dto)
		{
			var updatedEmp = employeeService.Update(dto.Id, dto.FirstName, dto.LastName, dto.PhoneNumber);

			if (updatedEmp)
			{
				return Response.Success(dto);
			}

			return Response.Error("Failed to update employee.", ErrorCode.EmployeeUpdateError, 400);
		}

		public async Task<IActionResult> CheckUserRole(AuthUserDTO token)
		{
			if (token is null || token.Role is null)
			{
				return Response.Error("Token is null", ErrorCode.InvalidRequest, 400);
			}

			if (token.Role.Contains("Client"))
			{
				return Response.Success(true);
			}
			var employee = employeeService.GetEmployeeByUsername(token.Nameid)?.User;
			if (employee is null)
				return Response.Error("Employee not found", ErrorCode.EmployeeNotFound, 404);

			var roles = await userManager.GetRolesAsync(employee);

			if (roles.Count != token.Role.Count)
			{
				return Response.Success(false);
			}

			foreach (var tokenRole in token.Role)
			{
				if (!roles.Contains(tokenRole))
				{
					return Response.Success(false);
				}
			}

			return Response.Success(true);
		}
	}
}
