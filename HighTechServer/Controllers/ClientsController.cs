using HighTech.Core.Entities;
using HighTech.Core.Services.Abstraction;
using HighTech.DTOs;
using HighTech.Mappers;
using HighTechServer;
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
			var clients = service.GetClients();
			var dtos = ClientMapper.ToDTOList(clients);

			return Response.Success(dtos);
		}

		[Authorize]
		[HttpPost]
		public IActionResult EditClient(ClientDTO dto)
		{
			var updatedClient = service.Update(dto.Id, dto.FirstName, dto.LastName, dto.PhoneNumber, dto.Address);

			if (updatedClient)
			{
				return Response.Success(dto);
			}

			return Response.Error("Failed to update client.", ErrorCode.ClientUpdateError, 400);
		}

		[Authorize]
		public IActionResult GetByUsername(string username)
		{
			if (username is null)
			{
				return Response.Error($"There is not a user with {username} username.", ErrorCode.ClientUsernameMissing, 400);
			}

			var client = service.GetClientByUsername(username);

			if (client is null)
			{
				return Response.Success<ClientDTO?>(null);
			}

			return Response.Success(ClientMapper.ToDTO(client));
		}

		[HttpPost]
		public async Task<ActionResult<ClientDTO>> Login(LoginDTO loginModel)
		{
			if (loginModel is null || loginModel.Username is null)
			{
				return Response.Error("Employee cannot be null!", ErrorCode.InvalidRequest, 400);
			}
			var user = await userManager.FindByNameAsync(loginModel.Username);
			if (user is null || loginModel.Password is null)
			{
				return Response.Error("Invalid username or password!", ErrorCode.InvalidCredentials, 401);
			}

			var result = await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);

			if (!result.Succeeded)
			{
				return Response.Error("Invalid username or password!", ErrorCode.InvalidCredentials, 401);
			}
			var userRoles = await userManager.GetRolesAsync(user);

			return Response.Success(CreateAuthUserDTO(user, userRoles));
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterDTO registerModel)
		{
			if (await userManager.Users.AnyAsync(u => u.Email == registerModel.Email!.ToLower()))
			{
				return Response.Error($"An existing account is using {registerModel.Email}. Please try with another email!", ErrorCode.EmailAlreadyExists, 400);
			}

			if (registerModel.Password == null || registerModel.ConfirmPassword == null)
			{
				return Response.Error("Password cannot be null", ErrorCode.InvalidRequest, 400);
			}
			if (registerModel.Password != registerModel.ConfirmPassword)
			{
				return Response.Error("Paswords don't match!", ErrorCode.PasswordMismatch, 400);
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

					return Response.Success(CreateAuthUserDTO(user, userRoles), 201);
				}
			}

			return Response.Error(string.Join(", ", result.Errors.Select(e => e.Description)), ErrorCode.ClientCreateError, 400);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
		{
			if (dto is null || dto.Username is null)
			{
				return Response.Error("Employee cannot be null!", ErrorCode.InvalidRequest, 400);
			}
			var user = await userManager.FindByNameAsync(dto.Username);
			if (user == null)
			{
				return Response.Error($"Unable to load user with username '{dto.Username}'.", ErrorCode.ClientNotFound, 404);
			}

			if (dto.NewPassword == null || dto.OldPassword == null)
			{
				return Response.Error("Password cannot be null", ErrorCode.InvalidRequest, 400);
			}
			if (dto.NewPassword == dto.OldPassword)
			{
				return Response.Error("Passwords doesn't match", ErrorCode.PasswordMismatch, 400);
			}

			var changePasswordResult = await userManager
				.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

			if (!changePasswordResult.Succeeded)
			{
				return Response.Error(string.Join(", ", changePasswordResult.Errors.Select(e => e.Description)), ErrorCode.PasswordChangeError, 400);
			}

			return Ok();
		}

		private AuthUserDTO CreateAuthUserDTO(AppUser user, IList<string> roles)
		{
			return new AuthUserDTO
			{
				Given_name = user.FirstName,
				Family_name = user.LastName,
				Role = roles,
				Nameid = user.UserName,
				Jwt = jwtService.CreateJWT(user, roles),
				Exp = new DateTime().AddDays(jwtService.ExpiresDays).Ticks
			};
		}
	}
}
