// IActionRepository.cs
using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Action = ems_back.Repo.Models.Action;

namespace ems_back.Repo.Interfaces
{
	public interface IActionRepository
	{
		Task<Action> GetByIdAsync(Guid id);
		Task<IEnumerable<Action>> GetByFlowAsync(Guid flowId);
		Task<Action> AddAsync(Action action);
		Task UpdateAsync(Action action);
		Task DeleteAsync(Guid id);
		Task<bool> ExistsAsync(Guid id);
	}
}