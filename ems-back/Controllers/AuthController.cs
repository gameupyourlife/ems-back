using ems_back.Repo.DTOs.Auth;
using ems_back.Repo.DTOs.Login;
using ems_back.Repo.DTOs.Register;
using ems_back.Repo.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ems_back.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly IUserService _userService;
		private readonly ILogger<AuthController> _logger;

		public AuthController(
			IAuthService authService,
			IUserService userService,
			ILogger<AuthController> logger)
		{
			_authService = authService;
			_userService = userService;
			_logger = logger;
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(new
					{
						Errors = ModelState.Values
							.SelectMany(v => v.Errors)
							.Select(e => new
							{
								Code = "Validation",
								Description = e.ErrorMessage ?? e.Exception?.Message
							})
					});
				}

				var result = await _authService.LoginAsync(request);

				if (!result.Success)
				{
					_logger.LogWarning("Login failed for {Email}", request.Email);
					return Unauthorized(new { result.Error });
				}

				// Handle automatic organization membership after successful login
				await _userService.HandleAutomaticOrganizationMembership(request.Email);

				return Ok(new { Token = result.Token });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during login for email: {Email}", request.Email);
				return StatusCode(500, new { Error = "An unexpected error occurred during login" });
			}
		}

		[HttpPost("register")]
		[AllowAnonymous]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(new
					{
						Errors = ModelState.Values
							.SelectMany(v => v.Errors)
							.Select(e => new
							{
								Code = "Validation",
								Description = e.ErrorMessage ?? e.Exception?.Message
							})
					});
				}

				var result = await _authService.RegisterAsync(request);

				if (!result.Success)
				{
					return BadRequest(new
					{
						Errors = result.Errors != null
							? result.Errors.Select(e => new { e.Code, e.Description })
							: new[] { new { Code = "General", Description = result.Error ?? "Registration failed" } }
					});
				}

				// Handle automatic organization membership after successful registration
				await _userService.HandleAutomaticOrganizationMembership(request.Email);

				var response = new
				{
					Message = "User registered successfully!",
					Token = result.Token

				
				};

				return Ok(response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Registration failed for email: {Email}", request.Email);
				return StatusCode(500, new
				{
					Error = "An unexpected error occurred during registration"
				});
			}
		}
	}
}