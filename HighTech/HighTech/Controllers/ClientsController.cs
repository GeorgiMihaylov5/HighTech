using HighTech.Abstraction;
using HighTech.DTOs;
using HighTech.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ClientsController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IClientService service;
        private readonly IJWTService jwtService;

        public ClientsController(SignInManager<AppUser> _signInManager,
            UserManager<AppUser> _userManager,
            IJWTService _jwtService, 
            IClientService _clientService)
        {
            signInManager = _signInManager;
            userManager = _userManager;
            service = _clientService;
            jwtService = _jwtService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult<UserDto>> Login(LoginDTO loginModel)
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

            return Json(CreateApplicationUserDto(user, userRoles));
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerModel)
        {

            if (await userManager.Users.AnyAsync(u => u.Email == registerModel.Email!.ToLower()))
            {
                return BadRequest($"An existing account is using {registerModel.Email}. Please try with another email!");
            };

            var user = new AppUser
            {
               FirstName = registerModel.FirstName,
               LastName = registerModel.LastName,
               Email = registerModel.Email,
               UserName = registerModel.Username
            };

            var result = await userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                var created = service.CreateClient(registerModel.Address ?? string.Empty, user.Id);

                if (created)
                {
                    await userManager.AddToRoleAsync(user, "Client");
                    await signInManager.SignInAsync(user, isPersistent: false);

                    var userRoles = await userManager.GetRolesAsync(user);

                    return Json(CreateApplicationUserDto(user, userRoles));
                }
            }

            return BadRequest(result.Errors);
        }

        private UserDto CreateApplicationUserDto(AppUser user, IList<string> roles)
        {
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = jwtService.CreateJWT(user, roles)
            };
        }

    }
}
