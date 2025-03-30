using ems_back.Repo.Data;
using ems_back.Repo.DTOs;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ems_back.Repo.Repository
{
	public class AgendaEntryRepository : IAgendaEntryRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public AgendaEntryRepository(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<AgendaEntryDto> GetByIdAsync(Guid id)
		{
			var entry = await _context.AgendaEntries
				.Include(a => a.Event)
				.AsNoTracking()
				.FirstOrDefaultAsync(a => a.Id == id);

			return _mapper.Map<AgendaEntryDto>(entry);
		}

		public async Task<IEnumerable<AgendaEntryDto>> GetByEventAsync(Guid eventId)
		{
			var entries = await _context.AgendaEntries
				.Where(a => a.EventId == eventId)
				.OrderBy(a => a.Start)
				.Include(a => a.Event)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<AgendaEntryDto>>(entries);
		}

		public async Task<IEnumerable<AgendaEntryDto>> GetUpcomingEntriesAsync(int days = 7)
		{
			var startDate = DateTime.UtcNow;
			var endDate = startDate.AddDays(days);

			var entries = await _context.AgendaEntries
				.Where(a => a.Start >= startDate && a.Start <= endDate)
				.OrderBy(a => a.Start)
				.Include(a => a.Event)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<AgendaEntryDto>>(entries);
		}

		public async Task<AgendaEntryDto> AddAsync(AgendaEntryCreateDto entryDto)
		{
			var entry = _mapper.Map<AgendaEntry>(entryDto);
			await _context.AgendaEntries.AddAsync(entry);
			await _context.SaveChangesAsync();
			return await GetByIdAsync(entry.Id);
		}

		public async Task<AgendaEntryDto> UpdateAsync(AgendaEntryUpdateDto entryDto)
		{
			var existingEntry = await _context.AgendaEntries.FindAsync(entryDto.Id);
			if (existingEntry == null)
				return null;

			_mapper.Map(entryDto, existingEntry);
			_context.AgendaEntries.Update(existingEntry);
			await _context.SaveChangesAsync();

			return await GetByIdAsync(entryDto.Id);
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var entry = await _context.AgendaEntries.FindAsync(id);
			if (entry == null)
				return false;

			_context.AgendaEntries.Remove(entry);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			return await _context.AgendaEntries.AnyAsync(a => a.Id == id);
		}
	}
}