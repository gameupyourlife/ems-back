using ems_back.Repo.Models;

namespace ems_back.Repo.Repositories;

public interface IUserRepository
{
	Task<IEnumerable<User>> GetAllUsersAsync();
	Task<User> GetUserByIdAsync(int id);
	Task<User> GetUserByEmailAsync(string email);
	Task AddUserAsync(User user);
	Task UpdateUserAsync(User user);
	Task DeleteUserAsync(int id);


}