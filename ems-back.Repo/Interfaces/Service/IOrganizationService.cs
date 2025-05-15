using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Organization;

namespace ems_back.Repo.Interfaces.Service
{
    public interface IOrganizationService

    {
        Task HandleAutomaticOrganizationMembership(string email);


        Task<OrganizationResponseDto> UpdateOrganizationAsync(
	        Guid id,
	        OrganizationUpdateDto organizationDto,
	        Guid updatedByUserId);

        // Add other service methods
       
	}
}
