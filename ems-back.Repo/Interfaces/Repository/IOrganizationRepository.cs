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

		//Task<OrganizationResponseDto> UpdateOrganizationAsync(Guid id, OrganizationUpdateDto organizationDto,
		//	Guid updatedByUserId);
		Task<bool> DeleteOrganizationAsync(Guid id, Guid updatedByUserId);

		//BasicSetups
		Task<Organization> GetByIdAsync(
			Guid id,
			Func<IQueryable<Organization>, IQueryable<Organization>> includes = null);

		Task UpdateAsync(Organization entity);



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

	}
}