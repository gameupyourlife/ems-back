using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;
namespace ems_back.Repo.Interfaces.Repository;

public interface IEventRepository
{
	Task<EventInfoDTO> GetByIdAsync(Guid id);
	Task<IEnumerable<EventInfoDTO>> GetAllEventsAsync();
	Task<IEnumerable<EventInfoDTO>> GetUpcomingEventsAsync(int days = 30);
	Task<IEnumerable<EventInfoDTO>> GetEventsByOrganizationAsync(Guid organizationId);
	Task<IEnumerable<EventInfoDTO>> GetEventsByCreatorAsync(Guid userId);
	Task<IEnumerable<EventInfoDTO>> GetEventsByCategoryAsync(int category);
	Task<IEnumerable<EventInfoDTO>> GetEventsByDateRangeAsync(DateTime start, DateTime end);
	Task<EventInfoDTO> AddAsync(EventCreateDto eventDto);
	Task<EventInfoDTO> UpdateAsync(EventInfoDTO eventDto);
	Task<EventInfoDTO> UpdateStatusAsync(Guid eventId, EventInfoDTO statusDto);
	Task<bool> DeleteAsync(Guid id);
	Task<bool> ExistsAsync(Guid id);
	Task<EventInfoDTO> GetEventWithAttendeesAsync(Guid eventId);
	Task<EventInfoDTO> GetEventWithAgendaAsync(Guid eventId);
	Task<EventInfoDTO> GetEventWithAllDetailsAsync(Guid eventId);
	Task<int> GetAttendeeCountAsync(Guid eventId);
}