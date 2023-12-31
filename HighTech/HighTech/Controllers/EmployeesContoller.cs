﻿using HighTech.Abstraction;
using HighTech.DTOs;
using HighTech.Models;
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

        public EmployeesController(UserManager<AppUser> _userManager,IEmployeeService _employeeService)
        {
            userManager = _userManager;
            employeeService = _employeeService;
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

            return Json(new EmployeeDTO()
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
            if (dto is null)
            {
                return BadRequest("User cannot be null!");
            }

            var user = await userManager.FindByIdAsync(dto.UserId);

            if (user == null)
            {
                return BadRequest($"Cannot find user with {dto.UserId} id!");
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
        public async Task<IActionResult> Demote(EmployeeDTO dto)
        {
            if (dto == null)
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

        public async Task<IActionResult> CheckUserRole(AuthUser token)
        {
            if (token is null)
            {
                return BadRequest("Token is null");
            }

            if (token.Role.Contains("Client"))
            {
                return Json(true);
            }

            var roles = await userManager.GetRolesAsync(employeeService.GetEmployeeByUsername(token.Nameid).User);

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
