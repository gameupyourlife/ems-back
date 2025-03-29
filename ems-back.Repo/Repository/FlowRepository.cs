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
	public class FlowRepository : IFlowRepository
	{
		private readonly ApplicationDbContext _context;

		public FlowRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Flow> AddAsync(Flow flow)
		{
			await _context.Flows.AddAsync(flow);
			await _context.SaveChangesAsync();
			return flow;
		}

		public async Task DeleteAsync(Guid id)
		{
			var flow = await GetByIdAsync(id);
			if (flow != null)
			{
				_context.Flows.Remove(flow);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			return await _context.Flows.AnyAsync(f => f.Id == id);
		}

		public async Task<IEnumerable<Flow>> GetAllActiveAsync()
		{
			return await _context.Flows
				.Where(f => f.IsActive)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<Flow> GetByIdAsync(Guid id)
		{
			return await _context.Flows
				.Include(f => f.Creator)
				.Include(f => f.Updater)
				.FirstOrDefaultAsync(f => f.Id == id);
		}

		public async Task<Flow> GetWithDetailsAsync(Guid id)
		{
			return await _context.Flows
				.Include(f => f.Triggers)
				.Include(f => f.Actions)
				.Include(f => f.Creator)
				.FirstOrDefaultAsync(f => f.Id == id);
		}

		public async Task ToggleStatusAsync(Guid id)
		{
			var flow = await GetByIdAsync(id);
			if (flow != null)
			{
				flow.IsActive = !flow.IsActive;
				await UpdateAsync(flow);
			}
		}

		public async Task UpdateAsync(Flow flow)
		{
			flow.UpdatedAt = DateTime.UtcNow;
			_context.Flows.Update(flow);
			await _context.SaveChangesAsync();
		}
	}
}
