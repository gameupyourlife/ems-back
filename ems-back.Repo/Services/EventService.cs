using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.Agenda;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
			IUserRepository userRepository,
            ILogger<EventService> logger)
		{
			_eventRepository = eventRepository;
			_logger = logger;
		}

        public async Task<IEnumerable<EventOverviewDto>> GetAllEventsAsync(Guid orgId)
		{
            if (!Guid.TryParse(orgId.ToString(), out Guid parsedOrgId))
            {
                _logger.LogWarning("Invalid organization ID format: {OrgId}", orgId);
                return Enumerable.Empty<EventOverviewDto>();
            }

            var events = await _eventRepository.GetAllEventsAsync(orgId);

			if (events == null)
			{
                _logger.LogWarning("No events found for organization with id {OrgId}", orgId);
                return Enumerable.Empty<EventOverviewDto>();
            }
			else
			{
				return events;
            }	
		}

        public async Task<EventInfoDTO> CreateEventAsync(EventCreateDto eventDto, Guid orgId)
        {

            var eventInfo = new EventInfoDTO
            {
                Title = eventDto.Title,
                OrganizationId = orgId,
                Description = eventDto.Description,
                Category = eventDto.Category,
                Location = eventDto.Location,
                Capacity = eventDto.Capacity,
                Image = eventDto.Image,
                Status = eventDto.Status,
                Start = eventDto.Start,
                End = eventDto.End,
                CreatedAt = eventDto.CreatedAt,
                UpdatedAt = eventDto.CreatedAt,
                CreatedBy = eventDto.CreatedBy,
                UpdatedBy = eventDto.UpdatedBy,
                AttendeeCount = 0
            };

            // Check if the event already exists

            var existingEvent = await _eventRepository.GetEventByTitleAndDateAsync(eventInfo.Title, eventInfo.Start, orgId);

            if (existingEvent != null)
            {
                _logger.LogWarning("Event with title {Title} and start date {StartDate} already exists", eventInfo.Title, eventInfo.Start);
                return null;
            }
            else
            {
                _logger.LogInformation("Creating new event with title {Title}", eventInfo.Title);

                return await _eventRepository.CreateEventAsync(eventDto);
            }
        }

        public async Task<EventInfoDTO> GetEventAsync(Guid orgId, Guid eventid)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(orgId, eventid);
            if (eventEntity == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventid);
            }
            return eventEntity;
        }

        public async Task<EventInfoDTO> UpdateEventAsync(Guid orgId, Guid eventId, EventInfoDTO eventDto)
        {

            // To Do: Manuell prüfen

            if (eventId != eventDto.Id)
            {
                _logger.LogWarning("Event ID mismatch: {EventId} != {DtoEventId}", eventId, eventDto.Id);
                return null;
            }
            var updatedEvent = await _eventRepository.UpdateEventAsync(eventDto);
            if (updatedEvent == null)
            {
                _logger.LogWarning("Failed to update event with id {EventId}", eventId);
            }
            return updatedEvent;
        }

        public async Task<bool> DeleteEventAsync(Guid orgId, Guid eventId)
        {

            // To Do: Manuell prüfen

            var eventExists = await _eventRepository.ExistsAsync(eventId);
            if (!eventExists)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                return false;
            }
            return await _eventRepository.DeleteAsync(eventId);
        }

        public async Task<List<EventAttendeeDto>> GetAllEventAttendeesAsync(Guid orgId, Guid eventId)
        {

            var attendeeList = await _eventRepository.GetAllEventAttendeesAsync(eventId);
            if (attendeeList == null)
            {
                _logger.LogWarning("No attendees found for event with id {EventId}", eventId);
                return new List<EventAttendeeDto>();
            }
            else
            {
                return attendeeList;
            }
        }

        public async Task<EventAttendeeDto> AddAttendeeToEventAsync(Guid orgId, Guid eventId, EventAttendeeDto attendeeDto)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAttendeeFromEventAsync(Guid eventId, Guid userId, Guid attendeeId)
        {

            // To Do: Manuell prüfen

            var attendeeExists = await _eventRepository.ExistsAsync(attendeeId);
            if (!attendeeExists)
            {
                _logger.LogWarning("Attendee with id {AttendeeId} not found", attendeeId);
                return false;
            }
            return await _eventRepository.DeleteAsync(attendeeId);
        }


		public async Task<List<AgendaEntry>> GetAgendaAsync(Guid id)
		{

            var agenda = await _eventRepository.GetAgendaWithEventAsync(id);
			if (agenda == null)
			{
				_logger.LogWarning("No agenda found for event with id {EventId}", id);
            }
            return agenda;
        }

        Task<EventInfoDTO> IEventService.CreateEventAsync(Guid orgId, EventCreateDto eventDto)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<EventAttendeeDto>> IEventService.GetAllEventAttendeesAsync(Guid orgId, Guid eventId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<AgendaEntryDto>> IEventService.GetAgendaAsync(Guid orgId, Guid eventId)
        {
            throw new NotImplementedException();
        }

        Task<AgendaEntryDto> IEventService.AddAgendaPointToEventAsync(Guid orgId, Guid eventId, AgendaEntryDto agendaEntryDto)
        {
            throw new NotImplementedException();
        }

        Task<AgendaEntryDto> IEventService.UpdateAgendaPointAsync(Guid orgId, Guid eventId, Guid agendaId, AgendaEntryDto agendaEntryDto)
        {
            throw new NotImplementedException();
        }

        Task<bool> IEventService.DeleteAgendaPointAsync(Guid orgId, Guid eventId, Guid agendaId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<FileDto>> IEventService.GetFilesFromEventAsync(Guid orgId, Guid eventId)
        {
            throw new NotImplementedException();
        }

        Task<FileDto> IEventService.AddFileToEventAsync(Guid orgId, Guid eventId, FileDto file)
        {
            throw new NotImplementedException();
        }

        Task<FileDto> IEventService.UpdateFileAsync(Guid orgId, Guid eventId, Guid fileId, FileDto file)
        {
            throw new NotImplementedException();
        }

        Task<FileDto> IEventService.DeleteFileAsync(Guid orgId, Guid eventId, Guid fileId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FileDto>> GetFilesFromEventAsync(Guid eventId)
        {
            return await _eventRepository.GetFilesFromEvent(eventId);
        }
    }
}
