using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.Password;
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
using AutoMapper;
using ems_back.Repo.Exceptions;
using ems_back.Repo.Repository;

namespace ems_back.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IOrganizationDomainRepository _orgDomainRepo;
        private readonly IOrganizationUserRepository _orgMembershipRepo;
        private readonly IMapper _mapper;


        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUserRepository userRepository,
               IOrganizationDomainRepository orgDomainRepo,
               IOrganizationUserRepository orgMembershipRepo,
            ILogger<UserService> logger, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _orgDomainRepo = orgDomainRepo;
            _orgMembershipRepo = orgMembershipRepo;
            _mapper = mapper;

            _logger = logger;
        }

        public async Task<bool> IsUserInOrgOrAdmin(Guid orgId, Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;
            if (user.Role == UserRole.Admin) return true;

            var orgUser = await _orgMembershipRepo.GetAsync(userId, orgId);
            if (orgUser == null) return false;

            return true;
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
                    throw new InvalidOperationException($"Email '{userDto.Email}' is already registered, use another Email please.");
                }

                // 2. Create user in Identity system
                var user = new User
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    UserName = userDto.Email,
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
                    Role = UserRole.User


                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email: {Email}", userDto.Email);
                throw;
            }
        }

        public async Task<UserResponseDto> UpdateUserAsync(Guid id, UserUpdateDto userDto)
        {
            try
            {
	            _logger.LogInformation("Starting user update for ID: {UserId}", id);

				// First update in repository
				var updatedUser = await _userRepository.UpdateUserAsync(id, userDto);
				_logger.LogDebug("Repository update completed for user ID: {UserId}", id);

				// Then update in Identity if needed
				var user = await _userManager.FindByIdAsync(id.ToString());
                if (user != null)
                {
	                _logger.LogDebug("Found Identity user for ID: {UserId}", id);
					user.FirstName = userDto.FirstName ?? user.FirstName;
                    user.LastName = userDto.LastName ?? user.LastName;
                    user.ProfilePicture = userDto.ProfilePicture ?? user.ProfilePicture;


                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
	                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
	                    _logger.LogError("Identity update failed for user ID: {UserId}. Errors: {Errors}", id, errors);
	                    throw new InvalidOperationException($"Identity update failed: {errors}");
                    }

                    _logger.LogDebug("Identity update successful for user ID: {UserId}", id);
                }
                else
                {
	                _logger.LogWarning("User not found in Identity store for ID: {UserId}", id);
                }

                _logger.LogInformation("Successfully updated user with ID: {UserId}", id);
                return updatedUser;
            }
            catch (Exception ex)
            {
	            _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
	            throw;
            }
		}

        //User Reset Password 
        public async Task ResetPasswordAsync(PasswordResetDto resetDto)
        {
            // Input validation (though this should also be handled at controller level)
            if (resetDto.NewPassword != resetDto.ConfirmPassword)
            {
                throw new ArgumentException("Password and confirmation do not match");
            }

            var user = await _userManager.FindByEmailAsync(resetDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Password reset attempt for non-existent email: {Email}", resetDto.Email);
                throw new KeyNotFoundException("User not found");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, token, resetDto.NewPassword);

            if (!resetResult.Succeeded)
            {
                var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                _logger.LogWarning("Password reset failed for {Email}. Errors: {Errors}",
                    resetDto.Email, errors);
                throw new InvalidOperationException($"Password reset failed: {errors}");
            }

            _logger.LogInformation("Password successfully reset for user {Email}", resetDto.Email);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            try
            {
                var userInfo = await _userRepository.GetUserByIdAsync(id);
                if (userInfo == null)
                {
                    _logger.LogWarning("Delete failed: User not found with ID: {UserId}", id);
                    return false;
                }

                if (await _userRepository.GetNumberOfOwnersAsync(id) <= 1 && userInfo.Role == UserRole.Owner)
                {
                    _logger.LogWarning("User {UserId} cannot be deleted because they are an owner", id);
                    throw new MissingRoleException("User cannot be deleted because they are an owner");
                }

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
                var organizations = await _userRepository.GetUserOrganizationsAsync(userId);

                if (!organizations.Any())
                {
                    _logger.LogWarning("No organizations found for user {UserId}", userId);
                    return Enumerable.Empty<OrganizationDto>();
                }

                var result = _mapper.Map<IEnumerable<OrganizationDto>>(organizations);
                _logger.LogInformation("Retrieved {OrganizationCount} organizations for user {UserId}",
                    result.Count(), userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving organizations for user {UserId}", userId);
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
                UserResponseDto userEntity;

                if (userId != null)
                {
                    userEntity = await _userRepository.GetUserByIdAsync(userId.Value);

                }
                else
                {
                    userEntity = await _userRepository.GetUserByEmailAsync(email);
                }

                var orgId = userEntity.Organization.Id;
                if (await _userRepository.GetNumberOfOwnersAsync(orgId) <= 1)
                {
                    _logger.LogWarning("User {UserId} cannot be deleted because he is the last owner", userId);
                    throw new MissingRoleException("User cannot be deleted because he is the last owner");
                }

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



        public async Task HandleAutomaticOrganizationMembership(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("Empty email provided");
                    return;
                }

                var domain = GetDomainFromEmail(email);
                var organizationDomain = await _orgDomainRepo.GetByDomainAsync(domain);

                if (organizationDomain == null)
                {
                    _logger.LogInformation("No organization found for domain {Domain}", domain);
                    return;
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("User with email {Email} not found", email);
                    return;
                }

                // Convert user.Id to Guid if needed
                var userId = user.Id is Guid ? (Guid)user.Id : Guid.Parse(user.Id.ToString());

                // Check if user is already a member for verification
                if (await _orgMembershipRepo.ExistsAsync(userId, organizationDomain.OrganizationId))
                {
                    _logger.LogInformation("User {UserId} already member of organization {OrgId}",
                        userId, organizationDomain.OrganizationId);
                    return;
                }

                // Create new User with required UserRole
                var newMembership = new OrganizationUser
                {
                    UserId = userId,
                    OrganizationId = organizationDomain.OrganizationId,
                    UserRole = UserRole.User,
                    JoinedAt = DateTime.UtcNow
                };

                await _orgMembershipRepo.AddAsync(newMembership);
                _logger.LogInformation("Added user {UserId}  via domain {Domain} to organization {OrgId}",
                    userId, domain, organizationDomain.OrganizationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling automatic organization membership for email {Email}", email);
                throw;
            }
        }

        private string GetDomainFromEmail(string email)
        {
            try
            {
                var atIndex = email.IndexOf('@');
                if (atIndex < 0 || atIndex == email.Length - 1)
                {
                    throw new ArgumentException("Invalid email format");
                }
                return email[(atIndex + 1)..].ToLower();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract domain from email {Email}", email);
                throw;
            }
        }

        public async Task<bool> PerformRestrictedAdminAction(Guid userId)
        {
	        var user = await _userManager.FindByIdAsync(userId.ToString())
	                   ?? throw new InvalidOperationException($"User {userId} does not exist.");

	        if (!await _userManager.IsInRoleAsync(user, "Admin"))
		        throw new UnauthorizedAccessException("User is not autorised.");

	        return true;
        }


	}
}