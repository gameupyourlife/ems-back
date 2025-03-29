using ems_back.Repo.Models;

namespace ems_back.Repo.Repositories;

public interface IEventRepository
{
	// Basic CRUD
	Task<IEnumerable<Event>> GetAllEventsAsync();
	Task<Event> GetEventByIdAsync(Guid id);
	Task<Event> AddEventAsync(Event eventEntity);

	Task UpdateEventAsync(Event eventEntity);  
	Task DeleteEventAsync(Guid id);
	Task<bool> EventExistsAsync(Guid id);

	// Extended queries
	Task<IEnumerable<Event>> GetUpcomingEventsAsync(int days = 30);
	Task<IEnumerable<Event>> GetEventsByOrganizationAsync(Guid organizationId);
	Task<int> GetAttendeeCountAsync(Guid eventId);

	// New methods based on Event class
	Task<IEnumerable<Event>> GetEventsByCreatorAsync(Guid userId);
	Task<IEnumerable<Event>> GetEventsByCategoryAsync(EventCategory category);

	// Navigation property loading
	Task<Event> GetEventWithAttendeesAsync(Guid eventId);
	Task<Event> GetEventWithAgendaAsync(Guid eventId);
	Task<Event> GetEventWithAllDetailsAsync(Guid eventId);  // Includes creator/updater/attendees/agenda
}