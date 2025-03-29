using ems_back.Repo.Models;

namespace ems_back.Repo.Interfaces;

public interface IUserRepository
{
	Task<IEnumerable<User>> GetAllUsersAsync();
	Task<User> GetUserByIdAsync(Guid id);
	Task<User> GetUserByEmailAsync(string email);
	Task AddUserAsync(User user);
	Task UpdateUserAsync(User user);
	Task DeleteUserAsync(Guid id);
	Task<bool> UserExistsAsync(Guid id);
	Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);
	Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
	Task<IEnumerable<User>> GetUsersByOrganizationAsync(Guid organizationId);


	Task<IEnumerable<Organization>> GetUserOrganizationsAsync(Guid userId);
	Task<UserRole> GetUserRoleAsync(Guid userId);
	Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId);

}