using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

public class OrganizationService : IOrganizationService
{
	private readonly IOrganizationRepository _organizationRepository;
	private readonly IOrganizationUserRepository _orgUserRepository;
	private readonly IOrganizationDomainRepository _orgDomainRepository;
	private readonly UserManager<User> _userManager;
	private readonly ILogger<OrganizationService> _logger;
	private readonly IMapper _mapper;

	public OrganizationService(
		IOrganizationRepository organizationRepository,
		IOrganizationUserRepository orgUserRepository,
		IOrganizationDomainRepository orgDomainRepository,
		UserManager<User> userManager, IMapper mapper,
		ILogger<OrganizationService> logger)
	{
		_organizationRepository = organizationRepository;
		_orgUserRepository = orgUserRepository;
		_orgDomainRepository = orgDomainRepository;
		_userManager = userManager;
		_logger = logger;
		_mapper = mapper;
	}

	// Existing methods remain unchanged
	public async Task<IEnumerable<OrganizationResponseDto>> GetAllOrganizationsAsync()
	{
		return await _organizationRepository.GetAllOrganizationsAsync();
	}

	public async Task<OrganizationResponseDto> GetOrganizationByIdAsync(Guid id)
	{
		return await _organizationRepository.GetOrganizationByIdAsync(id);
	}

	public async Task<IEnumerable<OrganizationOverviewDto>> GetOrganizationsByUserAsync(Guid userId)
	{
		return await _organizationRepository.GetOrganizationsByUserAsync(userId);
	}

	public async Task<int> GetMemberCountAsync(Guid id)
	{
		return await _organizationRepository.GetMemberCountAsync(id);
	}

	public async Task<OrganizationResponseDto> CreateOrganizationAsync(OrganizationCreateDto organizationDto)
	{
		return await _organizationRepository.CreateOrganizationAsync(organizationDto);
	}

	public async Task<OrganizationResponseDto> UpdateOrganizationAsync(
		Guid id,
		OrganizationUpdateDto organizationDto,
		Guid updatedByUserId)
	{
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
	public async Task<bool> DeleteOrganizationAsync(Guid id, Guid updatedByUserId)
	{
		return await _organizationRepository.DeleteOrganizationAsync(id, updatedByUserId);
	}



	// Enhanced automatic membership handling
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
}