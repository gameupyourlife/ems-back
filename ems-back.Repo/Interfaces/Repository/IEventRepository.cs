using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;
namespace ems_back.Repo.Interfaces.Repository;

public interface IEventRepository
{
	Task<EventDetailsDto> GetByIdAsync(Guid orgId, Guid id);
	Task<IEnumerable<EventOverviewDto>> GetAllEventsAsync(Guid orgId);
	Task<IEnumerable<EventInfoDTO>> GetUpcomingEventsAsync(int days = 30);
	Task<IEnumerable<EventInfoDTO>> GetEventsByOrganizationAsync(Guid organizationId);
	Task<IEnumerable<EventInfoDTO>> GetEventsByCreatorAsync(Guid userId);
	Task<IEnumerable<EventInfoDTO>> GetEventsByCategoryAsync(int category);
	Task<IEnumerable<EventInfoDTO>> GetEventsByDateRangeAsync(DateTime start, DateTime end);
	Task<EventDetailsDto> AddAsync(EventCreateDto eventDto);
	Task<EventDetailsDto> UpdateAsync(EventInfoDTO eventDto);
	Task<EventDetailsDto> UpdateStatusAsync(Guid eventId, EventInfoDTO statusDto);
	Task<bool> DeleteAsync(Guid id);
	Task<bool> ExistsAsync(Guid id);
	Task<List<EventAttendeeDto>> GetEventAttendeesAsync(Guid orgId, Guid eventId);
	Task<EventInfoDTO> GetEventWithAgendaAsync(Guid eventId);
	Task<EventInfoDTO> GetEventWithAllDetailsAsync(Guid eventId);
	Task<int> GetAttendeeCountAsync(Guid eventId);
}