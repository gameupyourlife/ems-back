using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.Agenda;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
namespace ems_back.Repo.Interfaces.Repository;

public interface IEventRepository
{
	Task<EventInfoDto> GetEventByIdAsync(Guid orgId, Guid id);
	Task<IEnumerable<EventOverviewDto>> GetAllEventsAsync(Guid orgId);
	Task<IEnumerable<EventInfoDto>> GetUpcomingEventsAsync(int days = 30);
	Task<IEnumerable<EventInfoDto>> GetEventsByOrganizationAsync(Guid organizationId);
	Task<IEnumerable<EventInfoDto>> GetEventsByCreatorAsync(Guid userId);
	Task<IEnumerable<EventInfoDto>> GetEventsByCategoryAsync(int category);
	Task<IEnumerable<EventInfoDto>> GetEventsByDateRangeAsync(DateTime start, DateTime end);
    Task<List<FileDto>> GetFilesFromEvent(Guid eventId);
    Task<Guid?> CreateEventAsync(EventInfoDto eventDto);
	Task<EventInfoDto> UpdateEventAsync(EventInfoDto eventDto);
	Task<EventInfoDto> UpdateStatusAsync(Guid eventId, EventInfoDto statusDto);
	Task<bool> DeleteAsync(Guid id);
	Task<bool> ExistsAsync(Guid id);
	Task<List<EventAttendeeDto>> GetAllEventAttendeesAsync(Guid eventId);
	Task<List<AgendaEntryDto>> GetAgendaWithEventAsync(Guid orgId, Guid eventId);
	Task<EventInfoDto> GetEventWithAllDetailsAsync(Guid eventId);
	Task<int> GetAttendeeCountAsync(Guid eventId);
    Task<EventOverviewDto> GetEventByTitleAndDateAsync(string title, DateTime start, Guid orgId);
}