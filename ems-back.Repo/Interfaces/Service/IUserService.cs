using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.Password;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Identity;

namespace ems_back.Repo.Interfaces.Service
{
	public interface IUserService
	{		Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
		Task<UserResponseDto> GetUserByIdAsync(Guid id);
		Task<IdentityResult> CreateUserAsync(User user, string password);
		Task<UserResponseDto> UpdateUserAsync(Guid id, UserUpdateDto userDto);
		Task ResetPasswordAsync(PasswordResetDto resetDto);
		Task<bool> UpdateUserRoleAsync(Guid userId, UserUpdateRoleDto userDto);
		Task<bool> DeleteUserAsync(Guid id);

		//specialized queries
		Task<IEnumerable<OrganizationDto>> GetUserOrganizationsAsync(Guid userId);






		Task<UserResponseDto> GetUserByEmailAsync(string email);

	


		Task<IEnumerable<UserResponseDto>> GetUsersByOrganizationAsync(Guid organizationId);

		Task<User> FindByEmailAsync(string email);

		Task<IdentityResult> AddToRoleAsync(User user, string role);
		Task<SignInResult> CheckPasswordSignInAsync(User user, string password);
		Task<bool> DeleteUserByIdOrEmailAsync(Guid? userId, string? email);

		Task HandleAutomaticOrganizationMembership(string email);
	}
}
