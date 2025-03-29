using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces
{
	public interface ITriggerRepository
	{
		Task<Trigger> GetByIdAsync(Guid id);
		Task<IEnumerable<Trigger>> GetByFlowAsync(Guid flowId);
		Task<IEnumerable<Trigger>> GetByTypeAsync(TriggerType type);
		Task<Trigger> AddAsync(Trigger trigger);
		Task UpdateAsync(Trigger trigger);
		Task DeleteAsync(Guid id);
		Task<bool> ExistsAsync(Guid id);
	}
}