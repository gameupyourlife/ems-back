using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.Agenda;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
namespace ems_back.Repo.Interfaces.Repository;

public interface IEventRepository
{
	Task<IEnumerable<EventOverviewDto>> GetAllEventsAsync(Guid orgId);
	Task<Guid> CreateEventAsync(EventInfoDto eventDto);
	Task<EventInfoDto> GetEventByIdAsync(Guid orgId, Guid eventId);
	Task<EventInfoDto> UpdateEventAsync(Guid orgId, Guid eventId, EventUpdateDto infoDto, Guid userId);
	Task<bool> DeleteEventAsync(Guid orgId, Guid eventId);
	Task<IEnumerable<EventAttendeeDto>> GetAllEventAttendeesAsync(Guid orgId, Guid eventId);
	Task<bool> AddAttendeeToEventAsync(EventAttendee attendee);
	Task<bool> RemoveAttendeeFromEventAsync(Guid eventId, Guid userId);
    // Task<EventOrganizer> GetEventOrganizerAsync(Guid orgId, Guid eventId, Guid organizerId);
    Task<bool> AddEventOrganizerAsync(Guid orgId, Guid eventId, Guid organizerId);
    Task<bool> RemoveEventOrganizerAsync(Guid orgId, Guid eventId, Guid organizerId);
    Task<IEnumerable<AgendaEntryDto>> GetAgendaByEventIdAsync(Guid eventId);
	Task<Guid> AddAgendaPointToEventAsync(AgendaEntryDto agendaEntry);
	Task<AgendaEntryDto> UpdateAgendaPointAsync(Guid orgId, Guid eventId, Guid agendaId, AgendaEntryDto agendaEntry);
	Task<AgendaEntryDto> DeleteAgendaPointAsync(Guid orgId, Guid eventId, Guid agendaId);

    // Additional methods

    Task<IEnumerable<EventInfoDto>> GetUpcomingEventsAsync(int days = 30);
	Task<IEnumerable<EventInfoDto>> GetEventsByCategoryAsync(int category);
	Task<IEnumerable<EventInfoDto>> GetEventsByDateRangeAsync(DateTime start, DateTime end);
	Task<EventInfoDto> UpdateEventStatusAsync(Guid eventId, EventInfoDto statusDto);
	Task<int> GetAttendeeCountAsync(Guid eventId);
	Task<EventOverviewDto> GetEventByTitleAndDateAsync(string title, DateTime start, Guid orgId);
}