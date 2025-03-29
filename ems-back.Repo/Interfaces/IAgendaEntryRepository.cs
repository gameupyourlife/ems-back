using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces
{
	public interface IAgendaEntryRepository
	{
		Task<AgendaEntry> GetByIdAsync(Guid id);
		Task<IEnumerable<AgendaEntry>> GetByEventAsync(Guid eventId);
		Task<AgendaEntry> AddAsync(AgendaEntry entry);
		Task UpdateAsync(AgendaEntry entry);
		Task DeleteAsync(Guid id);
		Task<bool> ExistsAsync(Guid id);
		Task<IEnumerable<AgendaEntry>> GetUpcomingEntriesAsync(int days = 7);
	}
}