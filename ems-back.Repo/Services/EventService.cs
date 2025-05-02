using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services
{
	public class EventService : IEventService
	{
		private readonly IEventRepository _eventRepository;
		private readonly ILogger<EventService> _logger;

		public EventService(
			IEventRepository eventRepository,
			ILogger<EventService> logger)
		{
			_eventRepository = eventRepository;
			_logger = logger;
		}

		public async Task<IEnumerable<EventBasicDto>> GetAllEventsAsync()
		{
			return await _eventRepository.GetAllEventsAsync();
		}

		public async Task<IEnumerable<EventBasicDto>> GetUpcomingEventsAsync(int days = 30)
		{
			return await _eventRepository.GetUpcomingEventsAsync(days);
		}

		public async Task<EventBasicDetailedDto> GetEventByIdAsync(Guid id)
		{
			var eventEntity = await _eventRepository.GetByIdAsync(id);
			if (eventEntity == null)
			{
				_logger.LogWarning("Event with id {EventId} not found", id);
			}
			return eventEntity;
		}

		public async Task<EventBasicDetailedDto> GetEventWithAttendeesAsync(Guid id)
		{
			return await _eventRepository.GetEventWithAttendeesAsync(id);
		}

		public async Task<EventBasicDetailedDto> GetEventWithAgendaAsync(Guid id)
		{
			return await _eventRepository.GetEventWithAgendaAsync(id);
		}

		public async Task<EventBasicDetailedDto> GetEventWithAllDetailsAsync(Guid id)
		{
			return await _eventRepository.GetEventWithAllDetailsAsync(id);
		}

		public async Task<IEnumerable<EventBasicDto>> GetEventsByOrganizationAsync(Guid organizationId)
		{
			return await _eventRepository.GetEventsByOrganizationAsync(organizationId);
		}

		public async Task<IEnumerable<EventBasicDto>> GetEventsByCreatorAsync(Guid userId)
		{
			return await _eventRepository.GetEventsByCreatorAsync(userId);
		}

		public async Task<IEnumerable<EventBasicDto>> GetEventsByCategoryAsync(EventCategory category)
		{
			return await _eventRepository.GetEventsByCategoryAsync(category);
		}

		public async Task<IEnumerable<EventBasicDto>> GetEventsByDateRangeAsync(DateTime start, DateTime end)
		{
			return await _eventRepository.GetEventsByDateRangeAsync(start, end);
		}

		public async Task<EventBasicDetailedDto> CreateEventAsync(EventCreateDto eventDto)
		{
			return await _eventRepository.AddAsync(eventDto);
		}

		public async Task<bool> UpdateEventAsync(Guid id, EventUpdateDto eventDto)
		{
			if (id != eventDto.Id)
			{
				return false;
			}
			return await _eventRepository.UpdateAsync(eventDto) != null;
		}

		public async Task<EventBasicDetailedDto> UpdateEventStatusAsync(Guid id, EventStatusDto statusDto)
		{
			return await _eventRepository.UpdateStatusAsync(id, statusDto);
		}

		public async Task<bool> DeleteEventAsync(Guid id)
		{
			return await _eventRepository.DeleteAsync(id);
		}

		public async Task<int> GetAttendeeCountAsync(Guid id)
		{
			return await _eventRepository.GetAttendeeCountAsync(id);
		}
	}
}
