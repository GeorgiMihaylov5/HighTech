using HighTech.Core.Entities;
using HighTech.Core.Services.Abstraction;
using HighTech.DTOs;
using HighTech.Mappers;
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

			return Json(employeesDTO);
		}

		[Authorize(Roles = "Administrator,Employee")]
		public IActionResult GetByUsername(string username)
		{
			if (username is null)
			{
				return BadRequest($"There is not a user with {username} username.");
			}

			var employee = employeeService.GetEmployeeByUsername(username);

			if (employee is null)
			{
				return Json(null);
			}

			return Json(EmployeeMapper.ToDTO(employee));
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> CreateEmployee(EmployeeDTO dto)
		{
			if (dto is null || dto.Username is null)
			{
				return BadRequest("Employee cannot be null!");
			}

			var employee = await userManager.FindByNameAsync(dto.Username);

			if (employee is not null)
			{
				return BadRequest("Employee exists!");
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
				return BadRequest(result.Errors);
			}

			try
			{
				var emp = employeeService.CreateEmployee(dto.JobTitle, user.Id);
				dto.Id = emp.Id;

				userManager.AddToRoleAsync(user, "Employee").Wait();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

			return Json(dto);
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Promote(EmployeeDTO dto)
		{
			if (dto is null || dto.UserId is null)
			{
				return BadRequest("User cannot be null!");
			}

			var user = await userManager.FindByIdAsync(dto.UserId);

			if (user == null)
			{
				return BadRequest($"Cannot find user with {dto.UserId} id!");
			}

			if (await userManager.IsInRoleAsync(user, "Administrator"))
			{
				return BadRequest("User is already in admininstrator role!");
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
				return BadRequest("User cannot be null!");
			}
			var user = await userManager.FindByIdAsync(dto.UserId);

			if (user == null)
			{
				return BadRequest($"Cannot find user with {dto.UserId} id!");
			}

			if (!await userManager.IsInRoleAsync(user, "Administrator"))
			{
				return BadRequest("User isn't an administaror!");
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
				return Json(dto);
			}

			return BadRequest();
		}

		public async Task<IActionResult> CheckUserRole(AuthUserDTO token)
		{
			if (token is null || token.Role is null)
			{
				return BadRequest("Token is null");
			}

			if (token.Role.Contains("Client"))
			{
				return Json(true);
			}
			var employee = employeeService.GetEmployeeByUsername(token.Nameid)?.User;
			if (employee is null)
				return BadRequest("Employee not found");

			var roles = await userManager.GetRolesAsync(employee);

			if (roles.Count != token.Role.Count)
			{
				return Json(false);
			}

			foreach (var tokenRole in token.Role)
			{
				if (!roles.Contains(tokenRole))
				{
					return Json(false);
				}
			}

			return Json(true);
		}
	}
}
