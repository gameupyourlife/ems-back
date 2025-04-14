using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ems_back.Repo.Models;
using Microsoft.Extensions.Configuration;

namespace ems_back.Services
{
	public class AuthService
	{
		private readonly IConfiguration _config;
		private readonly UserManager<User> _userManager;

		public AuthService(
			IConfiguration config,
			UserManager<User> userManager)
		{
			_config = config;
			_userManager = userManager;
		}

		public async Task<string> GenerateToken(User user)
		{
			// 1. Get user roles from Identity
			var roles = await _userManager.GetRolesAsync(user);

			// 2. Create claims (embed user data in the token)
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Email, user.Email!),
				new Claim(ClaimTypes.Name, user.FullName)
			};

			// 3. Add roles to claims
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			// 4. Sign the token with the JWT key
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			// 5. Create the token
			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiryInMinutes"])),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}