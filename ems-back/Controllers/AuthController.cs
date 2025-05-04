using ems_back.Repo.DTOs.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Login;
using ems_back.Repo.DTOs.Register;
using ems_back.Repo.Interfaces.Service;

namespace ems_back.Controllers
{

	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly ILogger<AuthController> _logger;

		public AuthController(
			IAuthService authService,
			ILogger<AuthController> logger)
		{
			_authService = authService;
			_logger = logger;
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var result = await _authService.LoginAsync(request);

			if (!result.Success)
			{
				_logger.LogWarning("Login failed for {Email}", request.Email);
				return Unauthorized(new { result.Error });
			}

			return Ok(new { Token = result.Token });
		}

		[HttpPost("register")]
		[AllowAnonymous]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			// Input validation
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

			try
			{
				var result = await _authService.RegisterAsync(request);

				if (!result.Success)  // Changed from result.Successful to result.Success
				{
					return BadRequest(new
					{
						Errors = result.Errors != null
							? result.Errors.Select(e => new { e.Code, e.Description })
							: new[] { new { Code = "General", Description = result.Error ?? "Registration failed" } }
					});
				}

				var response = new
				{
					Message = "User registered successfully!",
				
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
