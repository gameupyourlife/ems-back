using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ems_back.Repo.DTOs.Domain;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Repository;
using System.Globalization;

namespace ems_back.Services
{
	public class OrganizationService : IOrganizationService
	{
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IOrganizationUserRepository _orgUserRepository;
		private readonly IOrganizationDomainRepository _orgDomainRepository;
		private readonly UserManager<User> _userManager;
		private readonly ILogger<OrganizationService> _logger;
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;

		public OrganizationService(
			IOrganizationRepository organizationRepository,
			IOrganizationUserRepository orgUserRepository,
			IOrganizationDomainRepository orgDomainRepository,
			IUserRepository userRepository,
			UserManager<User> userManager,
			IMapper mapper,
			ILogger<OrganizationService> logger)
		{
			_organizationRepository = organizationRepository;
			_orgUserRepository = orgUserRepository;
			_orgDomainRepository = orgDomainRepository;
			_userManager = userManager;
			_logger = logger;
			_mapper = mapper;
			_userRepository = userRepository;
		}

		public async Task<IEnumerable<OrganizationResponseDto>> GetAllOrganizationsAsync(Guid userId)
		{
			var user = await _userManager.FindByIdAsync(userId.ToString());
			if (user == null)
			{
				_logger.LogWarning("User {UserId} not found", userId);
				throw new UnauthorizedAccessException("User not found");
			}
			// Check if user is ADMIN
			var isAdmin = await _userManager.IsInRoleAsync(user, nameof(UserRole.Admin));
			if (!isAdmin)
			{
				_logger.LogWarning("User {userId} lacks permission to update organization",
					userId);
				throw new UnauthorizedAccessException("Insufficient permissions");
			}

			return await _organizationRepository.GetAllOrganizationsAsync();
		}

		public async Task<OrganizationResponseDto> GetOrganizationByIdAsync(Guid id)
		{
			return await _organizationRepository.GetOrganizationByIdAsync(id);
		}

		public async Task<OrganizationResponseDto> CreateOrganizationAsync(OrganizationCreateDto organizationDto)
		{
			if (await _organizationRepository.DomainExistsAsync(organizationDto.Domain))
			{
				throw new DomainConflictException("This domain is already registered to another organization");
			}

			return await _organizationRepository.CreateOrganizationAsync(organizationDto);
		}
		//Admin or owner: done
		public async Task<OrganizationResponseDto> UpdateOrganizationAsync(
			Guid id,
			OrganizationUpdateDto organizationDto,
			Guid updatedByUserId)
		{
			// First verify the user has permission
			var user = await _userManager.FindByIdAsync(updatedByUserId.ToString());
			if (user == null)
			{
				_logger.LogWarning("User {UserId} not found", updatedByUserId);
				throw new UnauthorizedAccessException("User not found");
			}
			// Check if user is ADMIN
			var isAdmin = await _userManager.IsInRoleAsync(user, nameof(UserRole.Admin));

			// If not admin, check if user is OWNER of this organization
			var isOwner = !isAdmin && await _orgUserRepository.IsUserOrganizationOwner(updatedByUserId, id);

			if (!isAdmin && !isOwner)
			{
				_logger.LogWarning("User {UserId} lacks permission to update organization {OrganizationId}",
					updatedByUserId, id);
				throw new UnauthorizedAccessException("Insufficient permissions");
			}

			// Proceed with the existing update logic
			var organization = await _organizationRepository.GetByIdAsync(id, includes: q => q
				.Include(o => o.Creator)
				.Include(o => o.Updater));

			if (organization == null)
			{
				_logger.LogWarning("Organization {OrganizationId} not found", id);
				return null;
			}

			_mapper.Map(organizationDto, organization);
			organization.UpdatedAt = DateTime.UtcNow;
			organization.UpdatedBy = updatedByUserId;

			try
			{
				await _organizationRepository.UpdateAsync(organization);
				return _mapper.Map<OrganizationResponseDto>(organization);
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Error updating organization {OrganizationId}", id);
				throw new ApplicationException("Failed to update organization", ex);
			}
		}

		//only Admin, maybe Owner with verification steps
		public async Task<bool> DeleteOrganizationAsync(Guid userId, Guid organizationId)
		{
			// First verify the user has permission
			var user = await _userManager.FindByIdAsync(userId.ToString());
			if (user == null)
			{
				_logger.LogWarning("User {UserId} not found", userId);
				throw new UnauthorizedAccessException("User not found");
			}
			// Check if user is ADMIN
			var isAdmin = await _userManager.IsInRoleAsync(user, nameof(UserRole.Admin));

			// If not admin, check if user is OWNER of this organization
			var isOwner = !isAdmin && await _orgUserRepository.IsUserOrganizationOwner(userId, organizationId);

			if (!isAdmin && !isOwner)
			{
				_logger.LogWarning("User {UserId} lacks permission to update organization {OrganizationId}",
					userId, organizationId);
				throw new UnauthorizedAccessException("Insufficient permissions");
			}
			return await _organizationRepository.DeleteOrganizationAsync(organizationId, userId);
		}
		//admin,owner, 
		public async Task<IEnumerable<string>> GetOrganizationDomainsAsync(Guid organizationId,Guid userId)
		{
			_logger.LogInformation("Attempting to get all domains of {OrgId} by user {UserId}", organizationId, userId);
			if (!await _organizationRepository.OrganizationExistsAsync(organizationId))

			{
				_logger.LogWarning("Organization {OrganizationId} not found", organizationId);
				return null;
			}

			var user = await _userManager.FindByIdAsync(userId.ToString());
			if (user == null)
			{
				_logger.LogWarning("User {UserId} not found", userId);
				throw new UnauthorizedAccessException();
			}

			var isAdmin = await _userManager.IsInRoleAsync(user, nameof(UserRole.Admin));
			var isOwner = await _orgUserRepository.IsUserOrganizationOwner(userId, organizationId);

			_logger.LogDebug("User {UserId} permissions - Admin: {IsAdmin}, Owner: {IsOwner}",
				userId, isAdmin, isOwner);

			if (!isAdmin && !isOwner)
			{
				_logger.LogWarning("User {UserId} lacks permissions to view all domains in this Organization {OrgId}",
					userId, organizationId);
				throw new UnauthorizedAccessException();
			}

			return await _organizationRepository.GetOrganizationDomainsAsync(organizationId);

		}
		//Admin or Owner
		public async Task<bool> AddDomainToOrganizationAsync(
			Guid organizationId,
			string domain,
			Guid userId)
		{
			_logger.LogInformation("Attempting to add domain {Domain} to org {OrgId} by user {UserId}",
				domain, organizationId, userId);

			// 1. Verify organization exists
			if (!await _organizationRepository.OrganizationExistsAsync(organizationId))
			{
				_logger.LogWarning("Organization {OrganizationId} not found", organizationId);
				return false;
			}

			// 2. Check user permissions
			var user = await _userManager.FindByIdAsync(userId.ToString());
			if (user == null)
			{
				_logger.LogWarning("User {UserId} not found", userId);
				throw new UnauthorizedAccessException();
			}

			var isAdmin = await _userManager.IsInRoleAsync(user, nameof(UserRole.Admin));
			var isOwner = await _orgUserRepository.IsUserOrganizationOwner(userId, organizationId);

			_logger.LogDebug("User {UserId} permissions - Admin: {IsAdmin}, Owner: {IsOwner}",
				userId, isAdmin, isOwner);

			if (!isAdmin && !isOwner)
			{
				_logger.LogWarning("User {UserId} lacks permissions to add domains to org {OrgId}",
					userId, organizationId);
				throw new UnauthorizedAccessException();
			}

			// 3. Check domain availability
			if (!await _organizationRepository.IsDomainAvailableAsync(domain, organizationId))
			{
				_logger.LogWarning("Domain {Domain} already exists", domain);
				return false;
			}

			// 4. Add domain
			var result = await _organizationRepository.AddDomainToOrganizationAsync(organizationId, domain);

			_logger.LogInformation("Domain {Domain} added to org {OrgId} by user {UserId}: {Result}",
				domain, organizationId, userId, result);

			return result;
		}
		//Owner or admin
		public async Task<IEnumerable<UserResponseDto>> GetUsersByOrganizationAsync(Guid organizationId)
		{
			return await _userRepository.GetUsersByOrganizationAsync(organizationId);
		}

		public async Task HandleAutomaticOrganizationMembership(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				_logger.LogWarning("Empty email provided for organization membership");
				return;
			}

			try
			{
				var domain = GetDomainFromEmail(email);
				var orgDomain = await _orgDomainRepository.GetByDomainAsync(domain);

				if (orgDomain == null)
				{
					_logger.LogDebug("No organization mapped to domain {Domain}", domain);
					return;
				}

				var user = await _userManager.FindByEmailAsync(email);
				if (user == null)
				{
					_logger.LogWarning("User not found for email {Email}", email);
					return;
				}

				if (await _orgUserRepository.ExistsAsync(user.Id, orgDomain.OrganizationId))
				{
					_logger.LogDebug("User {UserId} already belongs to organization {OrgId}",
						user.Id, orgDomain.OrganizationId);
					return;
				}

				await AddUserToOrganization(user.Id, orgDomain.OrganizationId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to handle automatic organization membership for {Email}", email);
				throw;
			}
		}

		private async Task AddUserToOrganization(Guid userId, Guid organizationId)
		{
			var membership = new OrganizationUser
			{
				UserId = userId,
				OrganizationId = organizationId,
				UserRole = UserRole.User,
				JoinedAt = DateTime.UtcNow
			};

			await _orgUserRepository.AddAsync(membership);
			_logger.LogInformation("Added user {UserId} to organization {OrgId}", userId, organizationId);
		}

		private string GetDomainFromEmail(string email)
		{
			var atIndex = email.IndexOf('@');
			if (atIndex < 0 || atIndex == email.Length - 1)
			{
				throw new ArgumentException($"Invalid email format: {email}");
			}
			return email[(atIndex + 1)..].ToLower();
		}

		public async Task<IEnumerable<UserResponseDto>> GetOrganizationMembersAsync(Guid organizationId)
		{
			try
			{
				_logger.LogInformation("Getting members for organization {OrganizationId}", organizationId);

				// First verify the organization exists
				if (!await _organizationRepository.OrganizationExistsAsync(organizationId))
				{
					_logger.LogWarning("Organization {OrganizationId} not found", organizationId);
					return Enumerable.Empty<UserResponseDto>();
				}

				// Get users through OrganizationUser relationships
				var members = await _orgUserRepository.GetUsersByOrganizationAsync(organizationId);

				_logger.LogDebug("Retrieved {Count} members for organization {OrganizationId}",
					members.Count(), organizationId);

				return members;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving members for organization {OrganizationId}", organizationId);
				throw;
			}
		}
	}


}