using ems_back.Repo.DTOs;
using ems_back.Repo.Models;

namespace ems_back.Repo.Interfaces;

public interface IEventRepository
{
	Task<EventBasicDetailedDto> GetByIdAsync(Guid id);
	Task<IEnumerable<EventBasicDto>> GetAllEventsAsync();
	Task<IEnumerable<EventBasicDto>> GetUpcomingEventsAsync(int days = 30);
	Task<IEnumerable<EventBasicDto>> GetEventsByOrganizationAsync(Guid organizationId);
	Task<IEnumerable<EventBasicDto>> GetEventsByCreatorAsync(Guid userId);
	Task<IEnumerable<EventBasicDto>> GetEventsByCategoryAsync(EventCategory category);
	Task<IEnumerable<EventBasicDto>> GetEventsByDateRangeAsync(DateTime start, DateTime end);
	Task<EventBasicDetailedDto> AddAsync(EventCreateDto eventDto);
	Task<EventBasicDetailedDto> UpdateAsync(EventUpdateDto eventDto);
	Task<EventBasicDetailedDto> UpdateStatusAsync(Guid eventId, EventStatusDto statusDto);
	Task<bool> DeleteAsync(Guid id);
	Task<bool> ExistsAsync(Guid id);
	Task<EventBasicDetailedDto> GetEventWithAttendeesAsync(Guid eventId);
	Task<EventBasicDetailedDto> GetEventWithAgendaAsync(Guid eventId);
	Task<EventBasicDetailedDto> GetEventWithAllDetailsAsync(Guid eventId);
	Task<int> GetAttendeeCountAsync(Guid eventId);
}