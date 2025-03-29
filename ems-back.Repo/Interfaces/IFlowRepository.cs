using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces
{
	public interface IFlowRepository
	{
		Task<Flow> GetByIdAsync(Guid id);
		Task<IEnumerable<Flow>> GetAllActiveAsync();
		Task<Flow> AddAsync(Flow flow);
		Task UpdateAsync(Flow flow);
		Task ToggleStatusAsync(Guid id);
		Task DeleteAsync(Guid id);
		Task<bool> ExistsAsync(Guid id);
		Task<Flow> GetWithDetailsAsync(Guid id);
	}
}