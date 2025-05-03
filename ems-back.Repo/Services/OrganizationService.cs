using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;

public class OrganizationService : IOrganizationService
	{
		private readonly IOrganizationRepository _organizationRepository;

		public OrganizationService(IOrganizationRepository organizationRepository)
		{
			_organizationRepository = organizationRepository;
		}

		public async Task<IEnumerable<OrganizationResponseDto>> GetAllOrganizationsAsync()
		{
			return await _organizationRepository.GetAllOrganizationsAsync();
		}

		public async Task<OrganizationResponseDto> GetOrganizationByIdAsync(Guid id)
		{
			return await _organizationRepository.GetOrganizationByIdAsync(id);
		}

		public async Task<IEnumerable<OrganizationDto>> GetOrganizationsByUserAsync(Guid userId)
		{
			return await _organizationRepository.GetOrganizationsByUserAsync(userId);
		}

		public async Task<int> GetMemberCountAsync(Guid id)
		{
			return await _organizationRepository.GetMemberCountAsync(id);
		}

		public async Task<OrganizationResponseDto> CreateOrganizationAsync(OrganizationCreateDto organizationDto)
		{
			// Add validation/business logic here if needed
			return await _organizationRepository.CreateOrganizationAsync(organizationDto);
		}

		public async Task<bool> UpdateOrganizationAsync(Guid id, OrganizationUpdateDto organizationDto)
		{
			return await _organizationRepository.UpdateOrganizationAsync(id, organizationDto) != null;
		}

		public async Task<bool> DeleteOrganizationAsync(Guid id)
		{
			return await _organizationRepository.DeleteOrganizationAsync(id);
		}
	}