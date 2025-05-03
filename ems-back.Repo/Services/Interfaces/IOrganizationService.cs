using ems_back.Repo.DTOs.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services.Interfaces
{
	public interface IOrganizationService
	{
		Task<IEnumerable<OrganizationResponseDto>> GetAllOrganizationsAsync();
		Task<OrganizationResponseDto> GetOrganizationByIdAsync(Guid id);
		Task<IEnumerable<OrganizationDto>> GetOrganizationsByUserAsync(Guid userId);
		Task<int> GetMemberCountAsync(Guid id);
		Task<OrganizationResponseDto> CreateOrganizationAsync(OrganizationCreateDto organizationDto);
		Task<bool> UpdateOrganizationAsync(Guid id, OrganizationUpdateDto organizationDto);
		Task<bool> DeleteOrganizationAsync(Guid id);
	}
}
