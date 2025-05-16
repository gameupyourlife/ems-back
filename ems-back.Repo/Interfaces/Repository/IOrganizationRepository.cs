using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
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
		Task<bool> DeleteOrganizationAsync(Guid id, Guid updatedByUserId);

		// Entity Operations
		Task<Organization> GetByIdAsync(
			Guid id,
			Func<IQueryable<Organization>, IQueryable<Organization>> includes = null);
		Task UpdateAsync(Organization entity);

		// Query Operations
		Task<OrganizationResponseDto> GetOrganizationByIdAsync(Guid id);
		Task<IEnumerable<OrganizationResponseDto>> GetAllOrganizationsAsync();
		Task<IEnumerable<OrganizationOverviewDto>> GetOrganizationsByUserAsync(Guid userId);
		

		// Domain Management
		Task<bool> AddDomainToOrganizationAsync(Guid organizationId, string domain);
		Task<IEnumerable<string>> GetOrganizationDomainsAsync(Guid organizationId);
		Task<bool> DomainExistsAsync(string domain);
		Task<bool> IsDomainAvailableAsync(string domain, Guid? organizationId = null);

		// Validation Operations
		Task<bool> OrganizationExistsAsync(Guid id);
		


	// Internal Use
	Task<Organization> GetOrganizationEntityAsync(Guid id);
    Task<OrganizationUserDto> GetOrganizationUserAsync(Guid orgId, Guid userId);
}