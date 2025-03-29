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
	public class TriggerRepository : ITriggerRepository
	{
		private readonly ApplicationDbContext _context;

		public TriggerRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Trigger> AddAsync(Trigger trigger)
		{
			await _context.Triggers.AddAsync(trigger);
			await _context.SaveChangesAsync();
			return trigger;
		}

		public async Task DeleteAsync(Guid id)
		{
			var trigger = await GetByIdAsync(id);
			if (trigger != null)
			{
				_context.Triggers.Remove(trigger);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			return await _context.Triggers.AnyAsync(t => t.Id == id);
		}

		public async Task<Trigger> GetByIdAsync(Guid id)
		{
			return await _context.Triggers
				.Include(t => t.Flow)
				.FirstOrDefaultAsync(t => t.Id == id);
		}

		public async Task<IEnumerable<Trigger>> GetByFlowAsync(Guid flowId)
		{
			return await _context.Triggers
				.Where(t => t.FlowId == flowId)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IEnumerable<Trigger>> GetByTypeAsync(TriggerType type)
		{
			return await _context.Triggers
				.Where(t => t.Type == type)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task UpdateAsync(Trigger trigger)
		{
			_context.Triggers.Update(trigger);
			await _context.SaveChangesAsync();
		}
	}
}
