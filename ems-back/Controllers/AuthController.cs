using ems_back.Repo.Models.Types;
using ems_back.Repo.Models;
using ems_back.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ems_back.Repo.Models;
using ems_back.Repo.DTOs.Register;
using RegisterRequest = ems_back.Repo.DTOs.Register.RegisterRequest;
using Microsoft.AspNetCore.Authorization;

namespace ems_back.Controllers
{

	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly AuthService _authService;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;

		public AuthController(
			AuthService authService,
			UserManager<User> userManager,
			SignInManager<User> signInManager)
		{
			_authService = authService;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		[Authorize] 
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
				return Unauthorized("Invalid email or password.");

			var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
			if (!result.Succeeded)
				return Unauthorized("Invalid email or password.");

			var token = await _authService.GenerateToken(user);
			return Ok(new { Token = token });
		}

		[HttpPost("register")]
		[AllowAnonymous]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			var user = new User
			{
				FirstName = request.FirstName,
				LastName = request.LastName,
				Email = request.Email,
				UserName = request.Email,
				Role = request.Role // Add this line to accept role from request
			};

			var result = await _userManager.CreateAsync(user, request.Password);

			if (!result.Succeeded)
				return BadRequest(result.Errors);

			// Assign the role after user creation
			await _userManager.AddToRoleAsync(user, request.Role.ToString());

			return Ok(new { Message = "User registered successfully!" });
		}
	}
}
