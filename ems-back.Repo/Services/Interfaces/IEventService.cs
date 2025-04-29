using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services.Interfaces
{
	public interface IEventService
	{
		Task<IEnumerable<EventBasicDto>> GetAllEventsAsync();
		Task<IEnumerable<EventBasicDto>> GetUpcomingEventsAsync(int days = 30);
		Task<EventBasicDetailedDto> GetEventByIdAsync(Guid id);
		Task<EventBasicDetailedDto> GetEventWithAttendeesAsync(Guid id);
		Task<EventBasicDetailedDto> GetEventWithAgendaAsync(Guid id);
		Task<EventBasicDetailedDto> GetEventWithAllDetailsAsync(Guid id);
		Task<IEnumerable<EventBasicDto>> GetEventsByOrganizationAsync(Guid organizationId);
		Task<IEnumerable<EventBasicDto>> GetEventsByCreatorAsync(Guid userId);
		Task<IEnumerable<EventBasicDto>> GetEventsByCategoryAsync(EventCategory category);
		Task<IEnumerable<EventBasicDto>> GetEventsByDateRangeAsync(DateTime start, DateTime end);
		Task<EventBasicDetailedDto> CreateEventAsync(EventCreateDto eventDto);
		Task<bool> UpdateEventAsync(Guid id, EventUpdateDto eventDto);
		Task<EventBasicDetailedDto> UpdateEventStatusAsync(Guid id, EventStatusDto statusDto);
		Task<bool> DeleteEventAsync(Guid id);
		Task<int> GetAttendeeCountAsync(Guid id);
	}
}
