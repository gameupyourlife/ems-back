// IActionRepository.cs
using ems_back.Repo.DTOs;
using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Action = ems_back.Repo.Models.Action;

namespace ems_back.Repo.Interfaces
{
	public interface IActionRepository
	{
		Task<ActionDetailedDto> GetByIdAsync(Guid id);
		Task<IEnumerable<ActionDto>> GetByFlowAsync(Guid flowId);
		Task<ActionDetailedDto> AddAsync(ActionCreateDto actionDto);
		Task<ActionDetailedDto> UpdateAsync(ActionUpdateDto actionDto);
		Task<bool> DeleteAsync(Guid id);
		Task<bool> ExistsAsync(Guid id);
	}
}