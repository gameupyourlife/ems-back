using ems_back.Repo.Models;

namespace ems_back.Repo.Repositories;

public interface IOrganizationRepository
{
	Task<IEnumerable<Organization>> GetAllOrganizationsAsync();
	Task<Organization> GetOrganizationByIdAsync(Guid id);
	Task AddOrganizationAsync(Organization organization);
	Task UpdateOrganizationAsync(Organization organization);
	Task DeleteOrganizationAsync(Guid id);
	Task<bool> OrganizationExistsAsync(Guid id);
	Task<IEnumerable<Organization>> GetOrganizationsByUserAsync(Guid userId);
	Task<int> GetMemberCountAsync(Guid organizationId);
}