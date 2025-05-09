using ems_back.Repo.Models.Types;
using ems_back.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ems_back.Repo.Interfaces.Service;

namespace ems_back.Repo.Services
{
	public class RoleService : IRoleService
	{
		private readonly RoleManager<IdentityRole<Guid>> _roleManager;
		private readonly ILogger<RoleService> _logger;

		public RoleService(RoleManager<IdentityRole<Guid>> roleManager, ILogger<RoleService> logger)
		{
			_roleManager = roleManager;
			_logger = logger;
		}

		public async Task EnsureRolesExist()
		{
			foreach (var role in Enum.GetNames(typeof(UserRole)))
			{
				if (!await _roleManager.RoleExistsAsync(role))
				{
					var newRole = new IdentityRole<Guid>(role)
					{
						// Explicitly set ID if needed
						Id = Guid.NewGuid()
					};

					var result = await _roleManager.CreateAsync(newRole);
					if (result.Succeeded)
					{
						_logger.LogInformation("Created role: {RoleName}", role);
					}
					else
					{
						_logger.LogError("Failed to create role {RoleName}: {Errors}",
							role, string.Join(", ", result.Errors.Select(e => e.Description)));
					}
				}
			}
		}

		public async Task<List<string>> GetAllRoles()
		{
			return await _roleManager.Roles
				.Select(r => r.Name)
				.ToListAsync();
		}
	}
}