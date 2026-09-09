using HighTech.Abstraction;
using HighTech.Common;
using HighTech.DTOs;
using HighTech.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Employee,Administrator")]
        public IActionResult GetAll()
        {
            var clients = service.GetClients().Select(client => new ClientDTO()
            {
                Id = client.Id,
                UserId = client.UserId,
                Username = client.User.UserName,
                Email = client.User.Email,
                FirstName = client.User.FirstName,
                LastName = client.User.LastName,
                Address = client.Address,
                PhoneNumber = client.User.PhoneNumber
            });

            return Response.ResultSuccess(clients);
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditClient(ClientDTO dto)
        {
            var validationErrors = ValidateClient(dto);

            if (validationErrors.Count > 0)
            {
                return Response.ResultFailure<ClientDTO>(string.Join(", ", validationErrors), 400);
            }

            var updatedClient = service.Update(dto.Id, dto.FirstName, dto.LastName, dto.PhoneNumber, dto.Address);

            if(updatedClient)
            {
                return Response.ResultSuccess(dto);
            }

            return Response.ResultFailure<ClientDTO>("Failed to update client.", 400);
        }

        [Authorize]
        public IActionResult GetByUsername(string username)
        {
            if (username is null)
            {
                return Response.ResultFailure<ClientDTO>($"There is not a user with {username} username.", 400);
            }

            var client = service.GetClientByUsername(username);

            if (client is null)
            {
                return Response.ResultFailure<ClientDTO>("Client not found.", 404);
            }

            return Response.ResultSuccess(new ClientDTO()
            {
                Id = client.Id,
                UserId = client.UserId,
                Username = client.User.UserName,
                Email = client.User.Email,
                FirstName = client.User.FirstName,
                LastName = client.User.LastName,
                Address = client.Address,
                PhoneNumber = client.User.PhoneNumber
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginModel)
        {
            var user = await userManager.FindByNameAsync(loginModel.Username);
            if (user is null)
            {
                return Response.ResultFailure<AuthUser>("Invalid username or password!", 401);
            }

            var result = await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);

            if (!result.Succeeded)
            {
                return Response.ResultFailure<AuthUser>("Invalid username or password!", 401);
            }
            var userRoles = await userManager.GetRolesAsync(user);

            return Response.ResultSuccess(CreateAuthUserDTO(user, userRoles));
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerModel)
        {
            var validationErrors = ValidateRegister(registerModel);

            if (validationErrors.Count > 0)
            {
                return Response.ResultFailure<AuthUser>(string.Join(", ", validationErrors), 400);
            }

            if (await userManager.Users.AnyAsync(u => u.Email == registerModel.Email!.ToLower()))
            {
                return Response.ResultFailure<AuthUser>($"An existing account is using {registerModel.Email}. Please try with another email!", 400);
            };

            if(registerModel.Password != registerModel.ConfirmPassword)
            {
                return Response.ResultFailure<AuthUser>("Passwords don't match!", 400);
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

                    return Response.ResultSuccess(CreateAuthUserDTO(user, userRoles));
                }
            }

            return Response.ResultFailure<AuthUser>(string.Join(", ", result.Errors.Select(e => e.Description)), 400);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            var user = await userManager.FindByNameAsync(dto.Username);
            if (user == null)
            {
                return Response.ResultFailure<bool>($"Unable to load user with username '{dto.Username}'.", 404);
            }

            if (dto.NewPassword == dto.OldPassword)
            {
                return Response.ResultFailure<bool>("Passwords doesn't match", 400);
            }

            var changePasswordResult = await userManager
                .ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                return Response.ResultFailure<bool>(string.Join(", ", changePasswordResult.Errors.Select(e => e.Description)), 400);
            }

            return Response.ResultSuccess(true);
        }

        private AuthUser CreateAuthUserDTO(AppUser user, IList<string> roles)
        {
            return new AuthUser
            {
                Given_name = user.FirstName,
                Family_name = user.LastName,
                Role = roles,
                Nameid = user.UserName,
                Jwt = jwtService.CreateJWT(user, roles),
                Exp = new DateTime().AddDays(jwtService.ExpiresDays).Ticks
            };
        }

        private static List<string> ValidateClient(ClientDTO dto)
        {
            var errors = new List<string>();

            if (dto is null)
            {
                errors.Add("Client cannot be null.");
                return errors;
            }

            errors.AddRange(StringLimits.ValidateUserText(dto.FirstName, dto.LastName, dto.Email, dto.Username, dto.PhoneNumber));
            StringLimits.AddMaxLengthError(errors, "Address", dto.Address, StringLimits.Address);

            return errors;
        }

        private static List<string> ValidateRegister(RegisterDTO dto)
        {
            var errors = new List<string>();

            if (dto is null)
            {
                errors.Add("Registration data is required.");
                return errors;
            }

            errors.AddRange(StringLimits.ValidateUserText(dto.FirstName, dto.LastName, dto.Email, dto.Username, dto.PhoneNumber));
            StringLimits.AddMaxLengthError(errors, "Address", dto.Address, StringLimits.Address);

            return errors;
        }

    }
}
