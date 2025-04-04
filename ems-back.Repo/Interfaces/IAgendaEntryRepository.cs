using ems_back.Repo.DTOs.Agency;
using ems_back.Repo.DTOs.Agenda;
using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces
{
    public interface IAgendaEntryRepository
	{
		Task<AgendaEntryDto> GetByIdAsync(Guid id);
		Task<IEnumerable<AgendaEntryDto>> GetByEventAsync(Guid eventId);
		Task<IEnumerable<AgendaEntryDto>> GetUpcomingEntriesAsync(int days = 7);
		Task<AgendaEntryDto> AddAsync(AgendaEntryCreateDto entryDto);
		Task<AgendaEntryDto> UpdateAsync(AgendaEntryUpdateDto entryDto);
		Task<bool> DeleteAsync(Guid id);
		Task<bool> ExistsAsync(Guid id);
	}
}