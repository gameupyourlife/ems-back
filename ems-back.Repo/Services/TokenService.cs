using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ems_back.Repo.Models;
using Microsoft.Extensions.Logging;
using ems_back.Repo.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using ems_back.Repo.Interfaces.Service;

namespace ems_back.Repo.Services
{

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("Jwt:Key", "JWT signing key cannot be null or empty");
            }

            var keyByteCount = Encoding.UTF8.GetByteCount(key);
            if (keyByteCount < 64)
            {
                _logger.LogError("JWT key is too short. Expected at least 64 bytes, got {KeyLength} bytes", keyByteCount);
                throw new ArgumentException($"JWT key must be at least 64 bytes (512 bits). Current length: {keyByteCount} bytes");
            }

            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha512);
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            try
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                if (!string.IsNullOrEmpty(user.UserName))
                {
                    claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = _signingCredentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                _logger.LogInformation("Generated JWT token for user {UserId}", user.Id);
                return await Task.FromResult(tokenHandler.WriteToken(token));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user {UserId}", user?.Id);
                throw new SecurityTokenException("Failed to generate authentication token", ex);
            }
        }
    }
}
