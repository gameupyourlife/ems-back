using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Flow;

namespace ems_back.Repo.Interfaces
{
    public interface IFlowRepository
	{
		Task<FlowResponseDto> GetByIdAsync(Guid id);
		Task<IEnumerable<FlowBasicDto>> GetAllActiveAsync();
		Task<FlowResponseDto> AddAsync(FlowCreateDto flowDto);

		Task<FlowResponseDto> UpdateAsync(Guid id, FlowUpdateDto flowDto);
		Task<FlowResponseDto> ToggleStatusAsync(Guid id, FlowStatusDto statusDto);
		Task<bool> DeleteAsync(Guid id);
		Task<bool> ExistsAsync(Guid id);
		Task<FlowDetailedDto> GetWithDetailsAsync(Guid id);
	}
}