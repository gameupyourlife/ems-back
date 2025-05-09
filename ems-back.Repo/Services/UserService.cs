using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ems_back.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUserRepository userRepository,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            try
            {
                return await _userRepository.GetAllUsersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<UserResponseDto> GetUserByIdAsync(Guid id)
        {
            try
            {
                return await _userRepository.GetUserByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", id);
                throw;
            }
        }

        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto userDto)
        {
            // Validate input
            if (userDto == null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }

            try
            {
                _logger.LogInformation("Attempting to create user with email: {Email}", userDto.Email);

                // 1. Check email uniqueness
                var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Create user failed - email already exists: {Email}", userDto.Email);
                    throw new InvalidOperationException($"Email '{userDto.Email}' is already registered.");
                }

                // 2. Create user in Identity system
                var user = new User
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    UserName = userDto.Email,
                    //Role = UserRole.User
                    // Default role
                };

                var identityResult = await _userManager.CreateAsync(user, userDto.Password);
                if (!identityResult.Succeeded)
                {
                    var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("User creation failed for {Email}. Errors: {Errors}",
                        userDto.Email, errors);
                    throw new InvalidOperationException($"User creation failed: {errors}");
                }

                // 3. Create user in application repository
                var repositoryDto = new UserCreateDto
                {

                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,

                };

                var createdUser = await _userRepository.CreateUserAsync(repositoryDto);

                _logger.LogInformation("Successfully created user with ID: {UserId}", user.Id);
                return new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,

                    //Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email: {Email}", userDto.Email);
                throw; // Re-throw to let controller handle it
            }
        }

        public async Task<UserResponseDto> UpdateUserAsync(Guid id, UserUpdateDto userDto)
        {
            try
            {
                // First update in repository
                var updatedUser = await _userRepository.UpdateUserAsync(id, userDto);

                // Then update in Identity if needed
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user != null)
                {
                    user.FirstName = userDto.FirstName ?? user.FirstName;
                    user.LastName = userDto.LastName ?? user.LastName;


                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                    }

                    // Update password if provided
                    if (!string.IsNullOrEmpty(userDto.NewPassword))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var passwordResult = await _userManager.ResetPasswordAsync(user, token, userDto.NewPassword);
                        if (!passwordResult.Succeeded)
                        {
                            throw new InvalidOperationException(
                                string.Join(", ", passwordResult.Errors.Select(e => e.Description)));
                        }
                    }
                }

                return updatedUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            try
            {
                // First delete from repository
                var result = await _userRepository.DeleteUserAsync(id);

                // Then delete from Identity
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user != null)
                {
                    var deleteResult = await _userManager.DeleteAsync(user);
                    if (!deleteResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join(", ", deleteResult.Errors.Select(e => e.Description)));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
                throw;
            }
        }

        public async Task<UserResponseDto> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _userRepository.GetUserByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                throw;
            }
        }

        public async Task<IEnumerable<OrganizationDto>> GetUserOrganizationsAsync(Guid userId)
        {
            try
            {
                return await _userRepository.GetUserOrganizationsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting organizations for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<UserRole> GetUserRoleAsync(Guid userId)
        {
            try
            {
                return await _userRepository.GetUserRoleAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<EventInfoDTO>> GetUserEventsAsync(Guid userId)
        {
            try
            {
                return await _userRepository.GetUserEventsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting events for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(UserRole role)
        {
            try
            {
                return await _userRepository.GetUsersByRoleAsync(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by role: {Role}", role);
                throw;
            }
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByOrganizationAsync(Guid organizationId)
        {
            try
            {
                return await _userRepository.GetUsersByOrganizationAsync(organizationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by organization: {OrganizationId}", organizationId);
                throw;
            }
        }

        // Additional Identity-related methods
        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<SignInResult> CheckPasswordSignInAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }

        public async Task<bool> DeleteUserByIdOrEmailAsync(Guid? userId, string? email)
        {
            try
            {
                User? user = null;
                if (userId.HasValue)
                {
                    user = await _userManager.FindByIdAsync(userId.Value.ToString());
                }

                else if (!string.IsNullOrEmpty(email))
                {
                    user = await _userManager.FindByEmailAsync(email);
                }

                if (user == null)
                {
                    _logger.LogWarning("Delete failed: User not found (UserId: {UserId}, Email: {Email})", userId, email);
                    return false;
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to delete user from Identity: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return false;

                }
                _logger.LogInformation("Successfully deleted user with ID: {UserId} and Email: {Email}", user.Id, user.Email);
                return true;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user by Id or Email (UserId: {UserId}, Email: {Email})", userId, email);
                throw;
            }
        }
    }
}