using ems_back.Repo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ems_back.Repo.Repositories
{
	public interface IRegistrationRepository
	{
		Task<IEnumerable<Registration>> GetAllRegistrationsAsync();
		Task<Registration> GetRegistrationByIdAsync(int id);
		Task<IEnumerable<Registration>> GetRegistrationsByUserIdAsync(int userId);
		Task<IEnumerable<Registration>> GetRegistrationsByOrganizationIdAsync(int organizationId);
		Task<IEnumerable<Registration>> GetRegistrationsByEventIdAsync(int eventId);
		Task AddRegistrationAsync(Registration registration);
		Task DeleteRegistrationAsync(int id);
	}
}