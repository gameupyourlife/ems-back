using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Repository
{
	public interface IOrganizationUserRepository
	{
		
			Task<OrganizationUser?> GetAsync(Guid userId, Guid organizationId);
			Task AddAsync(OrganizationUser membership);
			Task RemoveAsync(OrganizationUser membership);
			Task<bool> ExistsAsync(Guid userId, Guid organizationId);
		
	}
}
