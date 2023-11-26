using HighTech.Abstraction;
using HighTech.DTOs;
using HighTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        [Authorize]
        [HttpPost]
        public IActionResult EditClient(ClientDTO dto)
        {
            var updatedClient = service.Update(dto.Id, dto.FirstName, dto.LastName, dto.PhoneNumber, dto.Address);

            if(updatedClient)
            {
                return Json(dto);
            }

            return BadRequest();
        }

        [Authorize]
        public IActionResult GetByUsername(string username)
        {
            if (username is null)
            {
                return BadRequest($"There is not a user with {username} username.");
            }

            var client = service.GetClientByUsername(username);

            return Json(new ClientDTO()
            {
                Id = client.Id,
                Username = client.User.UserName,
                Email = client.User.Email,
                FirstName = client.User.FirstName,
                LastName = client.User.LastName,
                Address = client.Address,
                PhoneNumber = client.User.PhoneNumber
            });
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

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            var user = await userManager.FindByNameAsync(dto.Username);
            if (user == null)
            {
                return NotFound($"Unable to load user with username '{dto.Username}'.");
            }

            if (dto.NewPassword == dto.OldPassword)
            {
                return BadRequest("Passwords doesn't match");
            }

            var changePasswordResult = await userManager
                .ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                return BadRequest(changePasswordResult.Errors);
            }

            return Ok();
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
