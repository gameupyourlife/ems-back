
using ems_back.Repo.DTOs.Auth;
using ems_back.Repo.Models;

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Login;
using ems_back.Repo.DTOs.Register;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Identity;

namespace ems_back.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUserService _userService;
		private readonly ITokenService _tokenService;
		private readonly ILogger<AuthService> _logger;
		private readonly IOrganizationService _organizationService;
        private readonly UserManager<User> _userManager;
        public AuthService(
			IUserService userService,
			ITokenService tokenService,
			IOrganizationService organizationService,
			ILogger<AuthService> logger,
			UserManager<User> userManager)
		{
			_userService = userService;
			_tokenService = tokenService;
			_logger = logger;
			_organizationService = organizationService;
            _userManager = userManager;
        }

		public async Task<AuthResult> LoginAsync(LoginRequest request)
		{
			try
			{
				var user = await _userService.FindByEmailAsync(request.Email);
				if (user == null)
				{
					_logger.LogWarning("Login attempt with non-existent email: {Email}", request.Email);
					return AuthResult.Failure("Invalid email or password.");
				}

				var signInResult = await _userService.CheckPasswordSignInAsync(user, request.Password);
				if (!signInResult.Succeeded)
				{
					_logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
					return AuthResult.Failure("Invalid email or password.");
				}

				// Add automatic organization membership handling
				await _organizationService.HandleAutomaticOrganizationMembership(request.Email);

				var token = await _tokenService.GenerateTokenAsync(user);
				_logger.LogInformation("User {Email} logged in successfully", request.Email);
				return AuthResult.CreateSuccess(token);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during login for email: {Email}", request.Email);
				return AuthResult.Failure("An unexpected error occurred during login.");
			}
		}

		public async Task<AuthResult> RegisterAsync(RegisterRequest request)
		{
			try
			{
				var user = new User
				{
					FirstName = request.FirstName,
					LastName = request.LastName,
					Email = request.Email,
					UserName = request.Email
				};

				var createResult = await _userService.CreateUserAsync(user, request.Password);
				if (!createResult.Succeeded)
				{
					_logger.LogWarning("User registration failed for {Email}: {Errors}",
						request.Email, string.Join(", ", createResult.Errors));
					return AuthResult.Failure(createResult.Errors);
				}

				var roleResult = await _userService.AddToRoleAsync(user, request.Role.ToString());
				if (!roleResult.Succeeded)
				{
					_logger.LogWarning("Role assignment failed for {Email}: {Errors}",
						request.Email, string.Join(", ", roleResult.Errors));
					return AuthResult.Failure(roleResult.Errors);
				}

				// Add automatic organization membership handling
				await _organizationService.HandleAutomaticOrganizationMembership(request.Email);

				_logger.LogInformation("User {Email} registered successfully with role {Role}",
					request.Email, request.Role);

				var token = await _tokenService.GenerateTokenAsync(user);
				return AuthResult.CreateSuccess(token);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
				return AuthResult.Failure("An unexpected error occurred during registration.");
			}
		}

		public async Task<string> GenerateTokenAsync(User user)
		{
			try
			{
				return await _tokenService.GenerateTokenAsync(user);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating token for user: {UserId}", user.Id);
				throw; 
			}
		}
    }
}