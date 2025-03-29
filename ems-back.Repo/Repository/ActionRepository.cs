using ems_back.Repo.Data;
using ems_back.Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ems_back.Repo.Models;
using Action = ems_back.Repo.Models.Action;


namespace ems_back.Repo.Repository
{
	public class ActionRepository(ApplicationDbContext context) : IActionRepository
	{
		private readonly ApplicationDbContext _context = context;

		public async Task<Action> AddAsync(Action action)
		{
			await _context.Actions.AddAsync(action);
			await _context.SaveChangesAsync();
			return action;
		}

		public async Task DeleteAsync(Guid id)
		{
			var action = await GetByIdAsync(id);
			if (action != null)
			{
				_context.Actions.Remove(action);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			return await _context.Actions.AnyAsync(a => a.Id == id);
		}

		public async Task<Action> GetByIdAsync(Guid id)
		{
			return await _context.Actions
				.Include(a => a.Flow)
				.FirstOrDefaultAsync(a => a.Id == id);
		}

		public async Task<IEnumerable<Action>> GetByFlowAsync(Guid flowId)
		{
			return await _context.Actions
				.Where(a => a.FlowId == flowId)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task UpdateAsync(Action action)
		{
			_context.Actions.Update(action);
			await _context.SaveChangesAsync();
		}
	}
}
