using HighTech.Abstraction;
using HighTech.Areas.Identity.Pages.Account.Manage;
using HighTech.DTOs;
using HighTech.Models;
using HighTech.RequestModels;
using HighTech.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ClientsController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<ChangePasswordModel> logger;
        private readonly IClientService service;
        private readonly JWTService jwtService;
        private string[] roles = new string[1];

        public ClientsController(SignInManager<ApplicationUser> _signInManager,
            UserManager<ApplicationUser> _userManager,
            JWTService _jwtService,
            ILogger<ChangePasswordModel> _logger, 
            IClientService _clientService)
        {
            signInManager = _signInManager;
            logger = _logger;
            userManager = _userManager;
            service = _clientService;
            jwtService = _jwtService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult<UserDto>> Login(LoginModel loginModel)
        {
            var user = await userManager.FindByNameAsync(loginModel.Username);
            if (user is null)
            {
                return Unauthorized("Invalid username or password!");
            }

            var result = await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid username or password!");
            }
            var userRoles = await userManager.GetRolesAsync(user);
            roles = userRoles.ToArray();

            var a = CreateApplicationUserDto(user);
            return a;

            //try
            //{
            //    await signInManager.SignOutAsync();
            //}
            //catch
            //{

            //}
            //var result = await signInManager.PasswordSignInAsync(registerModel.Username, registerModel.Password, true, lockoutOnFailure: false);

            //if (!result.Succeeded)
            //{
            //    return BadRequest();
            //}
            //return Json(result);
        }

        public UserDto CreateApplicationUserDto(ApplicationUser user)
        {
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = jwtService.CreateJWT(user, roles)
            };
        }






        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {

            if (await userManager.Users.AnyAsync(u => u.Email == registerModel.Email!.ToLower()))
            {
                return BadRequest($"An existing account is using {registerModel.Email}. Please try with another email!");
            };

            var user = new ApplicationUser
            {
               FirstName = registerModel.FirstName,
               LastName = registerModel.LastName,
               Email = registerModel.Email,
               UserName = registerModel.Username
            };

            var result = await userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(user);

            

            if (result.Succeeded)
            {
                var created = service.CreateClient(registerModel.Address ?? string.Empty, user.Id);

                if (created)
                {
                    await userManager.AddToRoleAsync(user, "Client");
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return Json(user);
                }
            }

            return BadRequest();
        }

    }
}
