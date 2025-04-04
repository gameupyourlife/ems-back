using ems_back.Repo.Data;
using ems_back.Repo.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Repository
{
    public class EventRepository : IEventRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public EventRepository(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<EventBasicDetailedDto> GetByIdAsync(Guid id)
		{
			var eventEntity = await _context.Events
				.Include(e => e.Creator)
				.Include(e => e.Updater)
				.AsNoTracking()
				.FirstOrDefaultAsync(e => e.Id == id);

			return _mapper.Map<EventBasicDetailedDto>(eventEntity);
		}

		public async Task<IEnumerable<EventBasicDto>> GetAllEventsAsync()
		{
			var events = await _context.Events
				.Include(e => e.Attendees)
				.Include(e => e.AgendaItems)
				.Include(e => e.Creator)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventBasicDto>>(events);
		}

		public async Task<IEnumerable<EventBasicDto>> GetUpcomingEventsAsync(int days = 30)
		{
			var cutoffDate = DateTime.UtcNow.AddDays(days);
			var events = await _context.Events
				.Where(e => e.Start >= DateTime.UtcNow && e.Start <= cutoffDate)
				.OrderBy(e => e.Start)
				.Include(e => e.Creator)
				.Include(e => e.Attendees)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventBasicDto>>(events);
		}

		public async Task<IEnumerable<EventBasicDto>> GetEventsByOrganizationAsync(Guid organizationId)
		{
			var events = await _context.Events
				.Where(e => e.OrganizationId == organizationId)
				.Include(e => e.Creator)
				.Include(e => e.Attendees)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventBasicDto>>(events);
		}

		public async Task<IEnumerable<EventBasicDto>> GetEventsByCreatorAsync(Guid userId)
		{
			var events = await _context.Events
				.Where(e => e.CreatedBy == userId)
				.Include(e => e.Updater)
				.Include(e => e.Attendees)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventBasicDto>>(events);
		}

		public async Task<IEnumerable<EventBasicDto>> GetEventsByCategoryAsync(EventCategory category)
		{
			var events = await _context.Events
				.Where(e => e.Category == category)
				.Include(e => e.Creator)
				.Include(e => e.Attendees)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventBasicDto>>(events);
		}

		public async Task<IEnumerable<EventBasicDto>> GetEventsByDateRangeAsync(DateTime start, DateTime end)
		{
			var events = await _context.Events
				.Where(e => e.Start >= start && e.End <= end)
				.OrderBy(e => e.Start)
				.Include(e => e.Creator)
				.Include(e => e.Attendees)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventBasicDto>>(events);
		}

		public async Task<EventBasicDetailedDto> AddAsync(EventCreateDto eventDto)
		{
			var eventEntity = _mapper.Map<Event>(eventDto);
			eventEntity.CreatedAt = DateTime.UtcNow;
			eventEntity.UpdatedAt = DateTime.UtcNow;

			await _context.Events.AddAsync(eventEntity);
			await _context.SaveChangesAsync();

			return await GetByIdAsync(eventEntity.Id);
		}

		public async Task<EventBasicDetailedDto> UpdateAsync(EventUpdateDto eventDto)
		{
			var existingEvent = await _context.Events.FindAsync(eventDto.Id);
			if (existingEvent == null)
				return null;

			_mapper.Map(eventDto, existingEvent);
			existingEvent.UpdatedAt = DateTime.UtcNow;

			_context.Events.Update(existingEvent);
			await _context.SaveChangesAsync();

			return await GetByIdAsync(eventDto.Id);
		}

		public async Task<EventBasicDetailedDto> UpdateStatusAsync(Guid eventId, EventStatusDto statusDto)
		{
			var existingEvent = await _context.Events.FindAsync(eventId);
			if (existingEvent == null)
				return null;

			existingEvent.Status = statusDto.Status;
			existingEvent.UpdatedBy = statusDto.UpdatedBy;
			existingEvent.UpdatedAt = DateTime.UtcNow;

			_context.Events.Update(existingEvent);
			await _context.SaveChangesAsync();

			return await GetByIdAsync(eventId);
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var eventEntity = await _context.Events.FindAsync(id);
			if (eventEntity == null)
				return false;

			_context.Events.Remove(eventEntity);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			return await _context.Events.AnyAsync(e => e.Id == id);
		}

		public async Task<EventBasicDetailedDto> GetEventWithAttendeesAsync(Guid eventId)
		{
			var eventEntity = await _context.Events
				.Include(e => e.Attendees)
					.ThenInclude(a => a.User)
				.Include(e => e.Creator)
				.FirstOrDefaultAsync(e => e.Id == eventId);

			return _mapper.Map<EventBasicDetailedDto>(eventEntity);
		}

		public async Task<EventBasicDetailedDto> GetEventWithAgendaAsync(Guid eventId)
		{
			var eventEntity = await _context.Events
				.Include(e => e.AgendaItems)
				.Include(e => e.Creator)
				.FirstOrDefaultAsync(e => e.Id == eventId);

			return _mapper.Map<EventBasicDetailedDto>(eventEntity);
		}

		public async Task<EventBasicDetailedDto> GetEventWithAllDetailsAsync(Guid eventId)
		{
			var eventEntity = await _context.Events
				.Include(e => e.Creator)
				.Include(e => e.Updater)
				.Include(e => e.Attendees)
					.ThenInclude(a => a.User)
				.Include(e => e.AgendaItems)
				.FirstOrDefaultAsync(e => e.Id == eventId);

			return _mapper.Map<EventBasicDetailedDto>(eventEntity);
		}

		public async Task<int> GetAttendeeCountAsync(Guid eventId)
		{
			return await _context.EventAttendees
				.CountAsync(ea => ea.EventId == eventId);
		}
	}
}