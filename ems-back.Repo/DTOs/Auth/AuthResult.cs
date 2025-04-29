using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Auth
{
	public class AuthResult
	{
		public bool Success { get; set; }
		public string Token { get; set; }
		public string Error { get; set; }
		public IEnumerable<IdentityError> Errors { get; set; }

		// Renamed to CreateSuccess to avoid conflict
		public static AuthResult CreateSuccess(string token = null)
		{
			return new AuthResult { Success = true, Token = token };
		}

		public static AuthResult Failure(string error)
		{
			return new AuthResult { Success = false, Error = error };
		}

		public static AuthResult Failure(IEnumerable<IdentityError> errors)
		{
			return new AuthResult { Success = false, Errors = errors };
		}
	}
}
