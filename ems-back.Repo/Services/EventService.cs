using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Models.Types;
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
        private IEventRepository @object;

        public EventService(
			IEventRepository eventRepository,
			ILogger<EventService> logger)
		{
			_eventRepository = eventRepository;
			_logger = logger;
		}

        public async Task<IEnumerable<EventInfoDTO>> GetAllEventsAsync()
		{
			return await _eventRepository.GetAllEventsAsync();
		}

		public async Task<IEnumerable<EventInfoDTO>> GetUpcomingEventsAsync(int days = 30)
		{
			return await _eventRepository.GetUpcomingEventsAsync(days);
		}

		public async Task<EventInfoDTO> GetEventByIdAsync(Guid id)
		{
			var eventEntity = await _eventRepository.GetByIdAsync(id);
			if (eventEntity == null)
			{
				_logger.LogWarning("Event with id {EventId} not found", id);
			}
			return eventEntity;
		}

		public async Task<EventInfoDTO> GetEventWithAttendeesAsync(Guid id)
		{
			return await _eventRepository.GetEventWithAttendeesAsync(id);
		}

		public async Task<EventInfoDTO> GetEventWithAgendaAsync(Guid id)
		{
			return await _eventRepository.GetEventWithAgendaAsync(id);
		}

		public async Task<EventInfoDTO> GetEventWithAllDetailsAsync(Guid id)
		{
			return await _eventRepository.GetEventWithAllDetailsAsync(id);
		}

		public async Task<IEnumerable<EventInfoDTO>> GetEventsByOrganizationAsync(Guid organizationId)
		{
			return await _eventRepository.GetEventsByOrganizationAsync(organizationId);
		}

		public async Task<IEnumerable<EventInfoDTO>> GetEventsByCreatorAsync(Guid userId)
		{
			return await _eventRepository.GetEventsByCreatorAsync(userId);
		}

		public async Task<IEnumerable<EventInfoDTO>> GetEventsByCategoryAsync(int category)
		{
			return await _eventRepository.GetEventsByCategoryAsync(category);
		}

		public async Task<IEnumerable<EventInfoDTO>> GetEventsByDateRangeAsync(DateTime start, DateTime end)
		{
			return await _eventRepository.GetEventsByDateRangeAsync(start, end);
		}

		public async Task<EventInfoDTO> CreateEventAsync(EventCreateDto eventDto)
		{
			return await _eventRepository.AddAsync(eventDto);
		}

		public async Task<bool> UpdateEventAsync(Guid id, EventInfoDTO eventDto)
		{
			if (id != eventDto.Id)
			{
				return false;
			}
			return await _eventRepository.UpdateAsync(eventDto) != null;
		}

		public async Task<EventInfoDTO> UpdateEventStatusAsync(Guid id, EventInfoDTO statusDto)
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
