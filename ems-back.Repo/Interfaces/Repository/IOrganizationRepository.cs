using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Repository
{
	public interface IOrganizationRepository
	{
		// CRUD Operations
		Task<OrganizationResponseDto> CreateOrganizationAsync(OrganizationCreateDto organizationDto);
		Task<OrganizationResponseDto> UpdateOrganizationAsync(Guid id, OrganizationUpdateDto organizationDto);
		Task<bool> DeleteOrganizationAsync(Guid id);

		// Query Methods
		Task<OrganizationResponseDto> GetOrganizationByIdAsync(Guid id);
		Task<IEnumerable<OrganizationResponseDto>> GetAllOrganizationsAsync();
		Task<IEnumerable<OrganizationOverviewDto>> GetOrganizationsByUserAsync(Guid userId);

		// Domain Management new
		Task<bool> AddDomainToOrganizationAsync(Guid organizationId, string domain);
		Task<IEnumerable<string>> GetOrganizationDomainsAsync(Guid organizationId);
		Task<bool> DomainExistsAsync(string domain);
		Task<bool> IsDomainAvailableAsync(string domain, Guid? organizationId = null);

		// Utility Methods
		Task<bool> OrganizationExistsAsync(Guid id);
		Task<int> GetMemberCountAsync(Guid organizationId);

		// Internal Use
		Task<Organization> GetOrganizationEntityAsync(Guid id);
	}
}