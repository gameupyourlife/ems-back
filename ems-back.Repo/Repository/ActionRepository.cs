using ems_back.Repo.Data;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Action = ems_back.Repo.Models.Action;
using ems_back.Repo.DTOs.Action;

namespace ems_back.Repo.Repository
{
    public class ActionRepository : IActionRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public ActionRepository(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<ActionDetailedDto> GetByIdAsync(Guid id)
		{
			var action = await _context.Actions
				.Include(a => a.Flow)
				.AsNoTracking()
				.FirstOrDefaultAsync(a => a.Id == id);

			return _mapper.Map<ActionDetailedDto>(action);
		}

		public async Task<IEnumerable<ActionDto>> GetByFlowAsync(Guid flowId)
		{
			var actions = await _context.Actions
				.Where(a => a.FlowId == flowId)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<ActionDto>>(actions);
		}

		public async Task<ActionDetailedDto> AddAsync(ActionCreateDto actionDto)
		{
			var action = _mapper.Map<Action>(actionDto);
			action.CreatedAt = DateTime.UtcNow;

			await _context.Actions.AddAsync(action);
			await _context.SaveChangesAsync();

			return await GetByIdAsync(action.Id);
		}

		public async Task<ActionDetailedDto> UpdateAsync(ActionUpdateDto actionDto)
		{
			var existingAction = await _context.Actions.FindAsync(actionDto.Id);
			if (existingAction == null)
				return null;

			// Only update properties that were provided
			if (actionDto.Type.HasValue)
				existingAction.Type = actionDto.Type.Value.ToString();

			if (actionDto.Details != null)
				existingAction.Details = actionDto.Details;

			_context.Actions.Update(existingAction);
			await _context.SaveChangesAsync();

			return await GetByIdAsync(actionDto.Id);
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var action = await _context.Actions.FindAsync(id);
			if (action == null)
				return false;

			_context.Actions.Remove(action);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			return await _context.Actions.AnyAsync(a => a.Id == id);
		}
	}
}