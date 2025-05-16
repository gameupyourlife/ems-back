using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.Agenda;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Identity;
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
        private readonly IUserRepository _userRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ILogger<EventService> _logger;
        private readonly UserManager<User> _userManager;


        public EventService(
			IEventRepository eventRepository,
			IUserRepository userRepository,
            IOrganizationRepository organizationRepository,
            ILogger<EventService> logger,
            UserManager<User> userManager)
		{
			_eventRepository = eventRepository;
            _userRepository = userRepository;
            _organizationRepository = organizationRepository;
            _logger = logger;
            _userManager = userManager;
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

        public async Task<EventInfoDto> CreateEventAsync(Guid orgId, EventCreateDto eventDto, Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} does not exist", userId);
                throw new UnauthorizedAccessException("User not found");
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, UserRole.Admin.ToString());
            var isOwner = await _userManager.IsInRoleAsync(user, UserRole.Owner.ToString());
            var isOrganizer = await _userManager.IsInRoleAsync(user, UserRole.Organizer.ToString());

            if (!isAdmin && !isOwner && !isOrganizer)
            {
                _logger.LogWarning("User with id {UserId} is not authorized to create events", userId);
                throw new UnauthorizedAccessException("Insufficient permissions");
            }

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
            eventInfo.Id = eventId;
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

        public async Task<EventInfoDto> UpdateEventAsync(
            Guid orgId, Guid eventId, EventUpdateDto eventDto)
        {
            if (eventId != eventDto.Id || orgId != eventDto.OrganizationId)
            {
                return null;
            }

            return await _eventRepository.UpdateEventAsync(eventDto);
        }

        public async Task<bool> DeleteEventAsync(Guid orgId, Guid eventId)
        {
            return await _eventRepository.DeleteEventAsync(orgId, eventId);
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
            EventAttendeeCreateDto attendeeDto)
        {
            var user = await _userRepository.GetUserByIdAsync(attendeeDto.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with id {userId} does not exist", attendeeDto.UserId);
                return null;
            }

            var eventInfo = await _eventRepository.GetEventByIdAsync(orgId, eventId);

            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                return null;
            }

            if (eventInfo.OrganizationId != orgId)
            {
                _logger.LogWarning("Event with id {EventId} does not belong to organization with id {OrgId}", eventId, orgId);
                return null;
            }

            var orgUser = await _organizationRepository.GetOrganizationUserAsync(orgId, attendeeDto.UserId);

            if (orgUser == null)
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", attendeeDto.UserId, orgId);
                return null;
            }

            if (orgUser.OrganizationId != eventInfo.OrganizationId)
            {
                _logger.LogWarning("User with id {UserId} does not belong to organization with id {OrgId}", attendeeDto.UserId, orgId);
                return null;
            }

            var attendee = new EventAttendee
            {
                UserId = attendeeDto.UserId,
                EventId = eventId,
                RegisteredAt = DateTime.UtcNow,
                Status = AttendeeStatus.Pending,
            };

            var isCreated = await _eventRepository.AddAttendeeToEventAsync(attendee);
            
            if (!isCreated)
            {
                _logger.LogWarning("Failed to add attendee to event with id {EventId}", eventId);
                return null;
            }

            var attendeeInfo = new EventAttendeeDto
            {
                UserId = attendee.UserId,
                UserEmail = user.Email,
                UserName = user.FullName,
                Status = attendee.Status,
                ProfilePicture = user.ProfilePicture,
                RegisteredAt = attendee.RegisteredAt
            };

            _logger.LogInformation("Attendee added to event with id {EventId}", eventId);
            return attendeeInfo;
        }

        public async Task<bool> RemoveAttendeeFromEventAsync(Guid orgId, Guid eventId, Guid userId)
        {
            return await _eventRepository.RemoveAttendeeFromEventAsync(orgId, eventId, userId);
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
            AgendaEntryCreateDto agendaEntryDto)
        {
            var agendaEntry = new AgendaEntryDto
            {
                Title = agendaEntryDto.Title,
                Description = agendaEntryDto.Description,
                Start = agendaEntryDto.Start,
                End = agendaEntryDto.End,
                EventId = eventId,
            };

            var agendaId = await _eventRepository.AddAgendaPointToEventAsync(agendaEntry);
            if (agendaId == null)
            {
                _logger.LogWarning("Failed to add agenda point to event with id {EventId}", eventId);
                return null;
            }

            agendaEntry.Id = agendaId;
            _logger.LogInformation("Agenda point added to event with id {EventId}", eventId);
            return agendaEntry;
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
            return await _eventRepository.GetFilesFromEvent(orgId, eventId);
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
