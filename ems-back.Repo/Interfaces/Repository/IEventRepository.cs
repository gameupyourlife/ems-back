using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.Agenda;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
namespace ems_back.Repo.Interfaces.Repository;

public interface IEventRepository
{
	Task<IEnumerable<EventOverviewDto>> GetAllEventsAsync(Guid orgId);
	Task<Guid?> CreateEventAsync(EventInfoDto eventDto);
	Task<EventInfoDto> GetEventByIdAsync(Guid orgId, Guid eventId);
	Task<EventInfoDto> UpdateEventAsync(Guid orgId, Guid eventId, EventInfoDto eventDto);
	Task<bool> DeleteEventAsync(Guid orgId, Guid eventId);
	Task<IEnumerable<EventAttendeeDto>> GetAllEventAttendeesAsync(Guid orgId, Guid eventId);
    Task<EventAttendeeDto> AddAttendeeToEventAsync(Guid orgId, Guid eventId, EventAttendeeDto attendee);
    Task<bool> RemoveAttendeeFromEventAsync(Guid orgId, Guid eventId, Guid userId);
    Task<IEnumerable<AgendaEntryDto>> GetAgendaByEventIdAsync(Guid orgId, Guid eventId);
    Task<Guid> AddAgendaPointToEventAsync(AgendaEntryDto agendaEntry);
    Task<AgendaEntryDto> UpdateAgendaPointAsync(Guid orgId, Guid eventId, Guid agendaId, AgendaEntryDto agendaEntry);
    Task<AgendaEntryDto> DeleteAgendaPointAsync(Guid orgId, Guid eventId, Guid agendaId);
	Task<IEnumerable<FileDto>> GetFilesFromEvent(Guid orgId, Guid eventId);
	Task<FileDto> AddFileToEvent(Guid orgId, Guid eventId, FileDto file);
	Task<FileDto> UpdateFile(Guid orgId, Guid eventId, Guid fileId, FileDto file);
	Task<FileDto> RemoveFileFromEvent(Guid orgId, Guid eventId, Guid fileId);

    // Additional methods

    Task<IEnumerable<EventInfoDto>> GetUpcomingEventsAsync(int days = 30);
	Task<IEnumerable<EventInfoDto>> GetEventsByCategoryAsync(int category);
	Task<IEnumerable<EventInfoDto>> GetEventsByDateRangeAsync(DateTime start, DateTime end);
	Task<EventInfoDto> UpdateEventStatusAsync(Guid eventId, EventInfoDto statusDto);
	Task<int> GetAttendeeCountAsync(Guid eventId);
    Task<EventOverviewDto> GetEventByTitleAndDateAsync(string title, DateTime start, Guid orgId);
}