using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services.Interfaces
{
	public interface ITriggerService
	{
		Task<TriggerDetailedDto?> GetByIdAsync(Guid id);
		Task<IEnumerable<TriggerDto>> GetByFlowAsync(Guid flowId);
		Task<IEnumerable<TriggerDto>> GetByTypeAsync(TriggerType type);
		Task<TriggerDetailedDto> AddAsync(TriggerCreateDto triggerDto);
		Task<TriggerDetailedDto?> UpdateAsync(TriggerUpdateDto triggerDto);
		Task<bool> DeleteAsync(Guid id);
	}
}
