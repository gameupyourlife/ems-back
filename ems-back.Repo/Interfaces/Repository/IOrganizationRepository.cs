using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.Models;

namespace ems_back.Repo.Interfaces.Repository;

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

	// Utility Methods
	Task<bool> OrganizationExistsAsync(Guid id);
	Task<int> GetMemberCountAsync(Guid organizationId);

	// Internal Use
	Task<Organization> GetOrganizationEntityAsync(Guid id);
    Task<OrganizationUserDto> GetOrganizationUserAsync(Guid orgId, Guid userId);
}