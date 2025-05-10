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

        public async Task<EventInfoDto> CreateEventAsync(Guid orgId, EventCreateDto eventDto)
        {
            var eventInfo = new EventInfoDto
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
                UpdatedBy = eventDto.CreatedBy,
                AttendeeCount = 0
            };

            // Check if the event already exists

            var existingEvent = await _eventRepository.GetEventByTitleAndDateAsync(eventInfo.Title, eventInfo.Start, orgId);

            if (existingEvent != null)
            {
                _logger.LogWarning("Event with title {Title} and start date {StartDate} already exists", eventInfo.Title, eventInfo.Start);
                return null;
            }
            
            var eventId = await _eventRepository.CreateEventAsync(eventInfo);

            if (eventId == null)
            {
                _logger.LogWarning("Failed to create event with title {Title}", eventInfo.Title);
                return null;
            }

            eventInfo.Id = eventId.Value;
            return eventInfo;
        }

        public async Task<EventInfoDto> GetEventAsync(Guid orgId, Guid eventid)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(orgId, eventid);
            if (eventEntity == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventid);
            }
            return eventEntity;
        }

        public async Task<EventInfoDto> UpdateEventAsync(Guid orgId, Guid eventId, EventInfoDto eventDto)
        {
            throw new NotImplementedException("UpdateEventAsync is not implemented yet");
        }

        public async Task<bool> DeleteEventAsync(Guid orgId, Guid eventId)
        {
            throw new NotImplementedException("DeleteEventAsync is not implemented yet");
        }

        public async Task<IEnumerable<EventAttendeeDto>> GetAllEventAttendeesAsync(Guid orgId, Guid eventId)
        {

            var attendeeList = await _eventRepository.GetAllEventAttendeesAsync(orgId, eventId);
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

        public async Task<EventAttendeeDto> AddAttendeeToEventAsync(
            Guid orgId, 
            Guid eventId, 
            EventAttendeeDto attendeeDto)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAttendeeFromEventAsync(Guid eventId, Guid userId, Guid attendeeId)
        {
            throw new NotImplementedException("RemoveAttendeeFromEventAsync is not implemented yet");
        }


		public async Task<IEnumerable<AgendaEntryDto>> GetAgendaAsync(Guid orgId, Guid eventId)
		{

            var agenda = await _eventRepository.GetAgendaByEventIdAsync(orgId, eventId);
			if (agenda == null)
			{
				_logger.LogWarning("No agenda found for event with id {EventId}", eventId);
            }
            return agenda;
        }

        public async Task<AgendaEntryDto> AddAgendaPointToEventAsync(
            Guid orgId, 
            Guid eventId, 
            AgendaEntryDto agendaEntryDto)
        {
            throw new NotImplementedException();
        }

        public async Task<AgendaEntryDto> UpdateAgendaPointAsync(
            Guid orgId, 
            Guid eventId, 
            Guid agendaId, 
            AgendaEntryDto agendaEntryDto)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAgendaPointAsync(Guid orgId, Guid eventId, Guid agendaId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FileDto>> GetFilesFromEventAsync(Guid orgId, Guid eventId)
        {
            throw new NotImplementedException();
        }

        public async Task<FileDto> AddFileToEventAsync(Guid orgId, Guid eventId, FileDto file)
        {
            throw new NotImplementedException();
        }

        public async Task<FileDto> UpdateFileAsync(Guid orgId, Guid eventId, Guid fileId, FileDto file)
        {
            throw new NotImplementedException();
        }

        public async Task<FileDto> DeleteFileAsync(Guid orgId, Guid eventId, Guid fileId)
        {
            throw new NotImplementedException();
        }
    }
}
