using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services.Interfaces
{
	public interface IRoleService
	{
		/// <summary>
		/// Ensures all roles defined in the UserRole enum exist in the Identity system
		/// </summary>
		Task EnsureRolesExist();

		/// <summary>
		/// Retrieves all available roles from the Identity system
		/// </summary>
		/// <returns>List of role names</returns>
		Task<List<string>> GetAllRoles();
	}
}
