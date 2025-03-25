using ems_back.Repo.Models;

namespace ems_back.Repo.Repositories;

public interface IOrganizationRepository
{
	Task<IEnumerable<Organization>> GetAllOrganizationsAsync();
	Task<Organization> GetOrganizationByIdAsync(int id);
	Task AddOrganizationAsync(Organization organization);
	Task UpdateOrganizationAsync(Organization organization);
	Task DeleteOrganizationAsync(int id);

}