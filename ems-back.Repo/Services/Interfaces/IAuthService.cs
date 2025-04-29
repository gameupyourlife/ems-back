using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Auth;
using ems_back.Repo.DTOs.Login;
using ems_back.Repo.DTOs.Register;
using ems_back.Repo.Models;

namespace ems_back.Repo.Services.Interfaces
{
	public interface IAuthService
	{
		Task<string> GenerateTokenAsync(User user);
		Task<AuthResult> LoginAsync(LoginRequest request);
		Task<AuthResult> RegisterAsync(RegisterRequest request);
	}
}
