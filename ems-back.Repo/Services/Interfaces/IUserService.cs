using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services.Interfaces
{
	public interface IUserService
	{
		Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
		Task<UserResponseDto> GetUserByIdAsync(Guid id);
		Task<UserResponseDto> CreateUserAsync(UserCreateDto userDto);
		Task<UserResponseDto> UpdateUserAsync(Guid id, UserUpdateDto userDto);
		Task<bool> DeleteUserAsync(Guid id);
		Task<UserResponseDto> GetUserByEmailAsync(string email);
		Task<IEnumerable<OrganizationDto>> GetUserOrganizationsAsync(Guid userId);
		Task<UserRole> GetUserRoleAsync(Guid userId);
		Task<IEnumerable<EventResponseDto>> GetUserEventsAsync(Guid userId);
		Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(UserRole role);
		Task<IEnumerable<UserResponseDto>> GetUsersByOrganizationAsync(Guid organizationId);

		Task<User> FindByEmailAsync(string email);
		Task<IdentityResult> CreateUserAsync(User user, string password);
		Task<IdentityResult> AddToRoleAsync(User user, string role);
		Task<SignInResult> CheckPasswordSignInAsync(User user, string password);
	}
}
