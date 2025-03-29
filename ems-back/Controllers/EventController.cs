using ems_back.Repo.Data;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using Microsoft.EntityFrameworkCore;

public class EventRepository : IEventRepository
{
	private readonly ApplicationDbContext _context;

	public EventRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<Event>> GetAllEventsAsync()
	{
		return await _context.Events
			.Include(e => e.Creator)
			.Include(e => e.Updater)
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<Event> GetEventByIdAsync(Guid id)
	{
		return await _context.Events
			.Include(e => e.Creator)
			.Include(e => e.Updater)
			.FirstOrDefaultAsync(e => e.Id == id);
	}

	public async Task<Event> AddEventAsync(Event eventEntity)
	{
		// Ensure timestamps are fresh
		eventEntity.CreatedAt = DateTime.UtcNow;
		eventEntity.UpdatedAt = DateTime.UtcNow;

		await _context.Events.AddAsync(eventEntity);
		await _context.SaveChangesAsync();
		return eventEntity;
	}

	public async Task UpdateEventAsync(Event eventEntity)
	{
		eventEntity.UpdatedAt = DateTime.UtcNow;
		_context.Events.Update(eventEntity);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteEventAsync(Guid id)
	{
		var eventEntity = await _context.Events.FindAsync(id);
		if (eventEntity != null)
		{
			_context.Events.Remove(eventEntity);
			await _context.SaveChangesAsync();
		}
	}

	public async Task<bool> EventExistsAsync(Guid id)
	{
		return await _context.Events
			.AnyAsync(e => e.Id == id);
	}

	public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int days = 30)
	{
		var cutoffDate = DateTime.UtcNow.AddDays(days);
		return await _context.Events
			.Where(e => e.Start >= DateTime.UtcNow && e.Start <= cutoffDate)
			.OrderBy(e => e.Start)
			.Include(e => e.Creator)
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<IEnumerable<Event>> GetEventsByOrganizationAsync(Guid organizationId)
	{
		return await _context.Events
			.Where(e => e.Creator.OrganizationId == organizationId)
			.Include(e => e.Creator)
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<int> GetAttendeeCountAsync(Guid eventId)
	{
		return await _context.EventAttendees
			.CountAsync(ea => ea.EventId == eventId);
	}

	public async Task<IEnumerable<Event>> GetEventsByCreatorAsync(Guid userId)
	{
		return await _context.Events
			.Where(e => e.CreatedBy == userId)
			.Include(e => e.Updater)
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<IEnumerable<Event>> GetEventsByCategoryAsync(EventCategory category)
	{
		return await _context.Events
			.Where(e => e.Category == category)
			.Include(e => e.Creator)
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<Event> GetEventWithAttendeesAsync(Guid eventId)
	{
		return await _context.Events
			.Include(e => e.Attendees)
				.ThenInclude(a => a.User)
			.Include(e => e.Creator)
			.FirstOrDefaultAsync(e => e.Id == eventId);
	}

	public async Task<Event> GetEventWithAgendaAsync(Guid eventId)
	{
		return await _context.Events
			.Include(e => e.AgendaItems)
			.Include(e => e.Creator)
			.FirstOrDefaultAsync(e => e.Id == eventId);
	}

	public async Task<Event> GetEventWithAllDetailsAsync(Guid eventId)
	{
		return await _context.Events
			.Include(e => e.Creator)
			.Include(e => e.Updater)
			.Include(e => e.Attendees)
				.ThenInclude(a => a.User)
			.Include(e => e.AgendaItems)
			.FirstOrDefaultAsync(e => e.Id == eventId);
	}

	// Additional methods based on your Event class
	public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime start, DateTime end)
	{
		return await _context.Events
			.Where(e => e.Start >= start && e.End <= end)
			.OrderBy(e => e.Start)
			.Include(e => e.Creator)
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task UpdateEventStatusAsync(Guid eventId, EventStatus status)
	{
		var eventEntity = await _context.Events.FindAsync(eventId);
		if (eventEntity != null)
		{
			eventEntity.Status = status;
			eventEntity.UpdatedAt = DateTime.UtcNow;
			await _context.SaveChangesAsync();
		}
	}
}