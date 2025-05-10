using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Repository
{
    public interface IUserRepository
	{
		// Basic CRUD with DTOs
		Task<UserResponseDto> CreateUserAsync(UserCreateDto userDto);
		Task<UserResponseDto> UpdateUserAsync(Guid userId, UserUpdateDto userDto);
		Task<bool> DeleteUserAsync(Guid id);

		// Query methods
		Task<UserResponseDto> GetUserByIdAsync(Guid id);
		Task<UserResponseDto> GetUserByEmailAsync(string email);
		Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
		Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(UserRole role);
		Task<IEnumerable<UserResponseDto>> GetUsersByOrganizationAsync(Guid organizationId);

		// Utility methods
		Task<bool> UserExistsAsync(Guid id);
		Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);

		// Specialized queries
		Task<IEnumerable<OrganizationDto>> GetUserOrganizationsAsync(Guid userId);
		Task<UserRole> GetUserRoleAsync(Guid userId);
		Task<IEnumerable<EventInfoDto>> GetUserEventsAsync(Guid userId);

		// Internal use only (for authentication/authorization)
		Task<User> GetUserEntityByIdAsync(Guid id);
		Task<User> GetUserEntityByEmailAsync(string email);
    }
}