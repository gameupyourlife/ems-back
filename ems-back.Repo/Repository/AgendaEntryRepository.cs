using ems_back.Repo.Data;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Repository
{
	public class AgendaEntryRepository : IAgendaEntryRepository
	{
		private readonly ApplicationDbContext _context;

		public AgendaEntryRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<AgendaEntry> AddAsync(AgendaEntry entry)
		{
			await _context.AgendaEntries.AddAsync(entry);
			await _context.SaveChangesAsync();
			return entry;
		}

		public async Task DeleteAsync(Guid id)
		{
			var entry = await GetByIdAsync(id);
			if (entry != null)
			{
				_context.AgendaEntries.Remove(entry);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			return await _context.AgendaEntries.AnyAsync(a => a.Id == id);
		}

		public async Task<AgendaEntry> GetByIdAsync(Guid id)
		{
			return await _context.AgendaEntries
				.Include(a => a.Event)
				.FirstOrDefaultAsync(a => a.Id == id);
		}

		public async Task<IEnumerable<AgendaEntry>> GetByEventAsync(Guid eventId)
		{
			return await _context.AgendaEntries
				.Where(a => a.EventId == eventId)
				.OrderBy(a => a.Start)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IEnumerable<AgendaEntry>> GetUpcomingEntriesAsync(int days = 7)
		{
			var startDate = DateTime.UtcNow;
			var endDate = startDate.AddDays(days);

			return await _context.AgendaEntries
				.Where(a => a.Start >= startDate && a.Start <= endDate)
				.OrderBy(a => a.Start)
				.Include(a => a.Event)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task UpdateAsync(AgendaEntry entry)
		{
			_context.AgendaEntries.Update(entry);
			await _context.SaveChangesAsync();
		}
	}
}
