using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces
{
	public interface IOrganizationUserRepository
	{
		Task<OrganizationUser> GetByIdAsync(Guid id);
		Task<IEnumerable<OrganizationUser>> GetByOrganizationIdAsync(Guid organizationId);
		Task<IEnumerable<OrganizationUser>> GetByUserIdAsync(Guid userId);
		Task<OrganizationUser> GetByOrganizationAndUserIdAsync(Guid organizationId, Guid userId);
		Task AddAsync(OrganizationUser organizationUser);
		Task UpdateAsync(OrganizationUser organizationUser);
		Task RemoveAsync(Guid id);
		Task RemoveByOrganizationAndUserIdAsync(Guid organizationId, Guid userId);
		Task<bool> ExistsAsync(Guid organizationId, Guid userId);
		Task<int> GetMemberCountAsync(Guid organizationId);
		Task UpdateUserRoleAsync(Guid organizationId, Guid userId, UserRole newRole);

		/*
		   Copy
		   GET    /api/organizationusers/organization/{orgId}    - Get all users in org
		   GET    /api/organizationusers/user/{userId}           - Get all orgs for user
		   GET    /api/organizationusers/{id}                    - Get specific relationship
		   POST   /api/organizationusers                         - Create new relationship
		   PUT    /api/organizationusers/{id}/role               - Update user role
		   DELETE /api/organizationusers/{id}                    - Remove relationship
		   GET    /api/organizationusers/organization/{orgId}/count - Get member count
		*/
	}
}

