using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Mvc;

namespace ems_back.Repo.Interfaces.Service
{
	public interface IOrganizationService

	{
		Task HandleAutomaticOrganizationMembership(string email);

		// Organization CRUD operations
		Task<IEnumerable<OrganizationResponseDto>> GetAllOrganizationsAsync(Guid userId);
		Task<OrganizationResponseDto> GetOrganizationByIdAsync(Guid id);
		Task<OrganizationResponseDto> CreateOrganizationAsync(OrganizationCreateDto organizationDto);
		Task<OrganizationResponseDto> UpdateOrganizationAsync(
			Guid id,
			OrganizationUpdateDto organizationDto,
			Guid updatedByUserId);
		Task<bool> DeleteOrganizationAsync(Guid userId, Guid organizationId);

		// Domain operations
		Task<IEnumerable<UserResponseDto>> GetOrganizationMembersAsync(Guid organizationId);
		Task<IEnumerable<string>> GetOrganizationDomainsAsync(Guid organizationId, Guid userId);

		Task<bool> AddDomainToOrganizationAsync(Guid orgId, string domain, Guid userId);

		// Member operations
		Task<IEnumerable<UserResponseDto>> GetUsersByOrganizationAsync(Guid organizationId);

		Task<RoleUpdateResult> UpdateUserRoleAsync(
			Guid currentUserId,
			Guid orgId,
			Guid targetUserId,
			UserRole newRole);
	}
}
