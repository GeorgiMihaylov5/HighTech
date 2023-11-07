using HighTech.Abstraction;
using HighTech.DTOs;
using HighTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class EmployeeContoller : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IEmployeeService employeeService;

        public EmployeeContoller(UserManager<AppUser> _userManager,IEmployeeService _employeeService)
        {
            userManager = _userManager;
            employeeService = _employeeService;
        }

        public async Task<IActionResult> GetAll()
        {
            var employeesDTO = employeeService
                .GetEmployees()
                .Select(e => new EmployeeDTO()
                {
                    Id = e.Id,
                    JobTitle = e.JobTitle,
                    User = new UserDTO()
                    {
                        Id = e.User.Id,
                        FirstName = e.User.FirstName,
                        LastName = e.User.LastName,
                        Email = e.User.Email,
                        PhoneNumber = e.User.PhoneNumber,
                        Username = e.User.UserName,
                    }
                })
                .OrderBy(x => x.User.FirstName)
                .ToList();

            var admins = (await userManager
                .GetUsersInRoleAsync("Administrator"))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.Id)
                .ToList();

            for (int i = 0; i < Math.Min(employeesDTO.Count, admins.Count); i++)
            {
                if (employeesDTO[i].User.Id == admins[i].Id)
                {
                    employeesDTO[i].IsAdmin = true;
                }
            }

            return Json(employeesDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeDTO dto)
        {
            var employee = await userManager.FindByNameAsync(dto.User.Username);

            if (employee is not null)
            {
                return BadRequest("Employee exists!");
            }

            var user = new AppUser
            {
                FirstName = dto.User.FirstName,
                LastName = dto.User.LastName,
                Email = dto.User.Email,
                UserName = dto.User.Username
            };

            var result = await userManager.CreateAsync(user, "employee123");

            if (result.Succeeded!)
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
        public async Task<IActionResult> Promote(string userId)
        {
            if (userId is null)
            {
                return BadRequest("User id cannot be null!");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest($"Cannot find user with {userId} id!");
            }

            if(await userManager.IsInRoleAsync(user, "Administrator"))
            {
                return BadRequest("User is already in admininstrator role!");
            }

            await userManager.AddToRoleAsync(user, "Administrator");
            
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Demote(string userId)
        {
            if (userId == null)
            {
                return BadRequest("User id cannot be null!");
            }
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest($"Cannot find user with {userId} id!");
            }

            if (!await userManager.IsInRoleAsync(user, "Administrator"))
            {
                return BadRequest("User isn't an administaror!");
            }

            await userManager.RemoveFromRoleAsync(user, "Administrator");

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(EmployeeDTO dto)
        {
            //TODO USER EDIT, NO EMPLOYEE OR CLIENT
            var isUpdated = employeeService.Update(dto.Id, dto.JobTitle);

            if (isUpdated)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
