using ems_back.Repo.Data;
using ems_back.Repo.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ems_back.Repo.Interfaces;
using ems_back.Repo.DTOs.Flow;

namespace ems_back.Repo.Repository
{
    public class FlowRepository : IFlowRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public FlowRepository(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<FlowResponseDto> GetByIdAsync(Guid id)
		{
			var flow = await _context.Flows
				.Include(f => f.Creator)
				.Include(f => f.Updater)
				.AsNoTracking()
				.FirstOrDefaultAsync(f => f.FlowId == id);

			return flow == null ? null : _mapper.Map<FlowResponseDto>(flow);
		}

		public async Task<IEnumerable<FlowBasicDto>> GetAllActiveAsync()
		{
			var flows = await _context.Flows
				.Where(f => f.IsActive)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<FlowBasicDto>>(flows);
		}

		public async Task<FlowResponseDto> AddAsync(FlowCreateDto flowDto)
		{
			var flow = _mapper.Map<Flow>(flowDto);
			flow.IsActive = true;
			flow.CreatedAt = DateTime.UtcNow;
			flow.UpdatedAt = DateTime.UtcNow;

			await _context.Flows.AddAsync(flow);
			await _context.SaveChangesAsync();

			// Reload with related entities for the response
			return await GetByIdAsync(flow.FlowId);
		}

		public async Task<FlowResponseDto> UpdateAsync(Guid id, FlowUpdateDto flowDto)
		{
			// 1. Find existing flow
			var existingFlow = await _context.Flows.FindAsync(id);
			if (existingFlow == null)
				return null;

			// 2. Update properties from DTO
			existingFlow.Name = flowDto.Name;
			existingFlow.Description = flowDto.Description;
			existingFlow.UpdatedBy = flowDto.UpdatedBy;
			existingFlow.UpdatedAt = DateTime.UtcNow;

			// 3. Save changes
			_context.Flows.Update(existingFlow);
			await _context.SaveChangesAsync();

			// 4. Return updated flow (reloaded with relationships)
			return await GetByIdAsync(id);
		}

		public async Task<FlowResponseDto> ToggleStatusAsync(Guid id, FlowStatusDto statusDto)
		{
			var flow = await _context.Flows.FindAsync(id);
			if (flow == null)
				return null;

			flow.IsActive = !flow.IsActive;
			flow.UpdatedBy = statusDto.UpdatedBy;
			flow.UpdatedAt = DateTime.UtcNow;

			_context.Flows.Update(flow);
			await _context.SaveChangesAsync();

			return await GetByIdAsync(id);
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var flow = await _context.Flows.FindAsync(id);
			if (flow == null)
				return false;

			_context.Flows.Remove(flow);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			return await _context.Flows.AnyAsync(f => f.FlowId == id);
		}

		public async Task<FlowDetailedDto> GetWithDetailsAsync(Guid id)
		{
			var flow = await _context.Flows
				.Include(f => f.Triggers)
				.Include(f => f.Actions)
				.Include(f => f.Creator)
				.Include(f => f.Updater)
				.AsNoTracking()
				.FirstOrDefaultAsync(f => f.FlowId == id);

			return flow == null ? null : _mapper.Map<FlowDetailedDto>(flow);
		}
	}
}