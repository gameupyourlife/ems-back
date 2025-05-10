using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
namespace ems_back.Repo.Interfaces.Repository;

public interface IEventRepository
{
	Task<EventInfoDTO> GetEventByIdAsync(Guid orgId, Guid id);
	Task<IEnumerable<EventOverviewDto>> GetAllEventsAsync(Guid orgId);
	Task<IEnumerable<EventInfoDTO>> GetUpcomingEventsAsync(int days = 30);
	Task<IEnumerable<EventInfoDTO>> GetEventsByOrganizationAsync(Guid organizationId);
	Task<IEnumerable<EventInfoDTO>> GetEventsByCreatorAsync(Guid userId);
	Task<IEnumerable<EventInfoDTO>> GetEventsByCategoryAsync(int category);
	Task<IEnumerable<EventInfoDTO>> GetEventsByDateRangeAsync(DateTime start, DateTime end);
    Task<List<FileDto>> GetFilesFromEvent(Guid eventId);
    Task<EventInfoDTO> CreateEventAsync(EventCreateDto eventDto);
	Task<EventInfoDTO> UpdateEventAsync(EventInfoDTO eventDto);
	Task<EventInfoDTO> UpdateStatusAsync(Guid eventId, EventInfoDTO statusDto);
	Task<bool> DeleteAsync(Guid id);
	Task<bool> ExistsAsync(Guid id);
	Task<List<EventAttendeeDto>> GetAllEventAttendeesAsync(Guid eventId);
	Task<List<AgendaEntry>> GetAgendaWithEventAsync(Guid eventId);
	Task<EventInfoDTO> GetEventWithAllDetailsAsync(Guid eventId);
	Task<int> GetAttendeeCountAsync(Guid eventId);
    Task<EventOverviewDto> GetEventByTitleAndDateAsync(string title, DateTime start, Guid orgId);
}