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
        public async Task<ActionResult<ClientDTO>> Login(LoginDTO loginModel)
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

            return Json(CreateAuthUserDTO(user, userRoles));
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerModel)
        {
            if (await userManager.Users.AnyAsync(u => u.Email == registerModel.Email!.ToLower()))
            {
                return BadRequest($"An existing account is using {registerModel.Email}. Please try with another email!");
            };

            if(registerModel.Password != registerModel.ConfirmPassword)
            {
                return BadRequest("Paswords don't match!");
            }

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
                var cleint = service.CreateClient(registerModel.Address, user.Id);

                if (cleint is not null)
                {
                    await userManager.AddToRoleAsync(user, "Client");
                    await signInManager.SignInAsync(user, isPersistent: false);

                    var userRoles = await userManager.GetRolesAsync(user);
                    
                    return Json(CreateAuthUserDTO(user, userRoles));
                }
            }

            return BadRequest(result.Errors);
        }

        private UserDTO CreateUserDTO(AppUser user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName,
            };
        }

        private AuthUser CreateAuthUserDTO(AppUser user, IList<string> roles)
        {
            return new AuthUser
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = roles,
                Jwt = jwtService.CreateJWT(user, roles),
                Exp = new DateTime().AddDays(jwtService.ExpiresDays).Ticks
            };
        }

    }
}
