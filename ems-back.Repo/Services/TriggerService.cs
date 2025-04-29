using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services
{
	public class TriggerService : ITriggerService
	{
		private readonly ITriggerRepository _triggerRepository;

		public TriggerService(ITriggerRepository triggerRepository)
		{
			_triggerRepository = triggerRepository;
		}

		public Task<TriggerDetailedDto?> GetByIdAsync(Guid id) => _triggerRepository.GetByIdAsync(id);
		public Task<IEnumerable<TriggerDto>> GetByFlowAsync(Guid flowId) => _triggerRepository.GetByFlowAsync(flowId);
		public Task<IEnumerable<TriggerDto>> GetByTypeAsync(TriggerType type) => _triggerRepository.GetByTypeAsync(type);
		public Task<TriggerDetailedDto> AddAsync(TriggerCreateDto triggerDto) => _triggerRepository.AddAsync(triggerDto);
		public Task<TriggerDetailedDto?> UpdateAsync(TriggerUpdateDto triggerDto) => _triggerRepository.UpdateAsync(triggerDto);
		public Task<bool> DeleteAsync(Guid id) => _triggerRepository.DeleteAsync(id);
	}
}
