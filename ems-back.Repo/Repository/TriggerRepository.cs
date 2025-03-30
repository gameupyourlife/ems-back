using AutoMapper;
using ems_back.Repo.Data;
using ems_back.Repo.DTOs;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using Microsoft.EntityFrameworkCore;

public class TriggerRepository : ITriggerRepository
{
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;

	public TriggerRepository(ApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<TriggerDetailedDto> GetByIdAsync(Guid id)
	{
		var trigger = await _context.Triggers
			.Include(t => t.Flow)
			.AsNoTracking()
			.FirstOrDefaultAsync(t => t.Id == id);

		return _mapper.Map<TriggerDetailedDto>(trigger);
	}

	public async Task<IEnumerable<TriggerDto>> GetByFlowAsync(Guid flowId)
	{
		var triggers = await _context.Triggers
			.Where(t => t.FlowId == flowId)
			.AsNoTracking()
			.ToListAsync();

		return _mapper.Map<IEnumerable<TriggerDto>>(triggers);
	}

	public async Task<IEnumerable<TriggerDto>> GetByTypeAsync(TriggerType type)
	{
		var triggers = await _context.Triggers
			.Where(t => t.Type == type)
			.AsNoTracking()
			.ToListAsync();

		return _mapper.Map<IEnumerable<TriggerDto>>(triggers);
	}

	public async Task<TriggerDetailedDto> AddAsync(TriggerCreateDto triggerDto)
	{
		var trigger = _mapper.Map<Trigger>(triggerDto);
		await _context.Triggers.AddAsync(trigger);
		await _context.SaveChangesAsync();
		return await GetByIdAsync(trigger.Id);
	}

	public async Task<TriggerDetailedDto> UpdateAsync(TriggerUpdateDto triggerDto)
	{
		var existingTrigger = await _context.Triggers.FindAsync(triggerDto.Id);
		if (existingTrigger == null) return null;

		_mapper.Map(triggerDto, existingTrigger);
		_context.Triggers.Update(existingTrigger);
		await _context.SaveChangesAsync();

		return await GetByIdAsync(triggerDto.Id);
	}

	public async Task<bool> DeleteAsync(Guid id)
	{
		var trigger = await _context.Triggers.FindAsync(id);
		if (trigger == null) return false;

		_context.Triggers.Remove(trigger);
		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<bool> ExistsAsync(Guid id)
	{
		return await _context.Triggers.AnyAsync(t => t.Id == id);
	}
}