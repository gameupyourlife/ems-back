using ems_back.Repo.DTOs.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services.Interfaces
{

	public interface IFlowService
	{
		Task<IEnumerable<FlowBasicDto>> GetAllActiveFlowsAsync();
		Task<FlowResponseDto> GetFlowByIdAsync(Guid id);
		Task<FlowDetailedDto> GetFlowDetailsAsync(Guid id);
		Task<FlowResponseDto> CreateFlowAsync(FlowCreateDto flowDto);
		Task<bool> UpdateFlowAsync(Guid id, FlowUpdateDto flowDto);
		Task<FlowResponseDto> ToggleFlowStatusAsync(Guid id, FlowStatusDto statusDto);
		Task<bool> DeleteFlowAsync(Guid id);
	}
}
