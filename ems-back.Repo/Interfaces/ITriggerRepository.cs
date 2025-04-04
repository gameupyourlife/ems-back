using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces
{
    public interface ITriggerRepository
	{

			Task<TriggerDetailedDto> GetByIdAsync(Guid id);
			Task<IEnumerable<TriggerDto>> GetByFlowAsync(Guid flowId);
			Task<IEnumerable<TriggerDto>> GetByTypeAsync(TriggerType type);
			Task<TriggerDetailedDto> AddAsync(TriggerCreateDto triggerDto);
			Task<TriggerDetailedDto> UpdateAsync(TriggerUpdateDto triggerDto);
			Task<bool> DeleteAsync(Guid id);
			Task<bool> ExistsAsync(Guid id);
		
	}
}