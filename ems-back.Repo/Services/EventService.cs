using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.Agenda;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
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
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly ILogger<EventService> _logger;
        private readonly UserManager<User> _userManager;


        public EventService(
			IEventRepository eventRepository,
			IUserRepository userRepository,
            IOrganizationUserRepository organizationUserRepository,
            ILogger<EventService> logger,
            UserManager<User> userManager)
		{
			_eventRepository = eventRepository;
            _userRepository = userRepository;
            _organizationUserRepository = organizationUserRepository;
            _logger = logger;
            _userManager = userManager;
        }

        private async Task<bool> IsAuthenticated(
            Guid userId, 
            bool shouldAdmin, 
            bool shouldOwner, 
            bool shouldOrganizer, 
            bool shouldEventOrganizer)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} does not exist", userId);
                return false;
            }

            if (shouldAdmin)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, UserRole.Admin.ToString());
                if (isAdmin) return true;
            }

            if (shouldOwner)
            {
                var isOwner = await _userManager.IsInRoleAsync(user, UserRole.Owner.ToString());
                if (isOwner) return true;
            }

            if (shouldOrganizer)
            {
                var isOrganizer = await _userManager.IsInRoleAsync(user, UserRole.Organizer.ToString());
                if (isOrganizer) return true;
            }

            if (shouldEventOrganizer)
            {
                var isEventOrganizer = await _userManager.IsInRoleAsync(user, UserRole.EventOrganizer.ToString());
                if (isEventOrganizer) return true;
            }

            return false;
        }

        private async Task<bool> IsUserInOrgOrAdmin(Guid orgId, Guid userId)
        {
            var user = await _organizationUserRepository.GetAsync(userId, orgId);
            if (user == null)
            {
                if (user.User.Role == UserRole.Admin) return true;
                return false;
            }
            
            return true;
        }

        public async Task<IEnumerable<EventOverviewDto>> GetAllEventsAsync(Guid orgId, Guid userId)
		{
            if (!await IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                return null;
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
            if (!await IsAuthenticated(userId, true, true, true, false))
            {
                _logger.LogWarning("User with id {UserId} is not authorized to create events", userId);
                return null;
            }

            if (!await IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                return null;
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

        public async Task<EventInfoDto> GetEventAsync(Guid orgId, Guid eventid, Guid userId)
        {
            if (!await IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                return null;
            }

            var eventEntity = await _eventRepository.GetEventByIdAsync(orgId, eventid);
            if (eventEntity == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventid);
            }
            return eventEntity;
        }

        public async Task<EventInfoDto> UpdateEventAsync(
            Guid orgId, 
            Guid eventId, 
            EventUpdateDto eventDto,
            Guid userId)
        {
            if (!await IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                return null;
            }

            if (!await IsAuthenticated(userId, true, true, true, true))
            {
                _logger.LogWarning("User with id {UserId} is not authorized to update events", userId);
                return null;
            }

            return await _eventRepository.UpdateEventAsync(orgId, eventId, eventDto, userId);
        }

        public async Task<bool> DeleteEventAsync(Guid orgId, Guid eventId, Guid userId)
        {
            if (!await IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                return false;
            }

            if (!await IsAuthenticated(userId, true, true, true, false))
            {
                _logger.LogWarning("User with id {UserId} is not authorized to delete events", userId);
                return false;
            }

            return await _eventRepository.DeleteEventAsync(orgId, eventId);
        }

        public async Task<IEnumerable<EventAttendeeDto>> GetAllEventAttendeesAsync(Guid orgId, Guid eventId)
        {
            if (!await IsUserInOrgOrAdmin(orgId, eventId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", eventId, orgId);
                return null;
            }

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
            EventAttendeeCreateDto attendeeDto,
            Guid userId)
        {
            if (!await IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                return null;
            }

            if (!await IsAuthenticated(userId, true, true, true, true))
            {
                _logger.LogWarning("User with id {UserId} is not authorized to add attendees to events", userId);
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

            if (eventInfo.AttendeeCount >= eventInfo.Capacity)
            {
                _logger.LogWarning("Event with id {EventId} is full", eventId);
                return null;
            }

            var attendee = new EventAttendee
            {
                UserId = attendeeDto.UserId,
                EventId = eventId,
                RegisteredAt = DateTime.UtcNow,
                Status = AttendeeStatus.Pending,
            };

            try
            {
                var isCreated = await _eventRepository.AddAttendeeToEventAsync(attendee);

                if (!isCreated)
                {
                    _logger.LogWarning("Failed to add attendee to event with id {EventId}", eventId);
                    return null;
                }
            }

            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error adding attendee to event with id {EventId}", eventId);
                return null;
            }

            var user = await _userRepository.GetUserByIdAsync(attendeeDto.UserId);
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

        public async Task<bool> RemoveAttendeeFromEventAsync(Guid orgId, Guid eventId, Guid attendeeId, Guid userId)
        {
            if (!await IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                return false;
            }

            if (!await IsAuthenticated(userId, true, true, true, true))
            {
                _logger.LogWarning("User with id {UserId} is not authorized to remove attendees from events", userId);
                return false;
            }

            var eventInfo = await _eventRepository.GetEventByIdAsync(orgId, eventId);
            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                return false;
            }

            return await _eventRepository.RemoveAttendeeFromEventAsync(eventId, attendeeId);
        }

        public async Task<bool> AddEventOrganizerAsync(Guid orgId, Guid eventId, Guid organizerId, Guid userId)
        {

           // To Do: Prüfen, ob ein Datensatz schon existiert
           // Überprüfen ob es bei anderen POST Befehlen auch schon abgeprüft wird

            if (!await IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                return false;
            }

            if (!await IsAuthenticated(userId, true, true, true, true))
            {
                _logger.LogWarning("User with id {UserId} is not authorized to add event organizers", userId);
                return false;
            }

            //var organizer = 

            return await _eventRepository.AddEventOrganizerAsync(orgId, eventId, organizerId);
        }

        public async Task<bool> RemoveEventOrganizerAsync(Guid orgId, Guid eventId, Guid organizerId, Guid userId)
        {
            if (!await IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                return false;
            }

            if (!await IsAuthenticated(userId, true, true, true, false))
            {
                _logger.LogWarning("User with id {UserId} is not authorized to add event organizers", userId);
                return false;
            }

            return await _eventRepository.RemoveEventOrganizerAsync(orgId, eventId, organizerId);
        }

        public async Task<IEnumerable<AgendaEntryDto>> GetAgendaAsync(Guid orgId, Guid eventId, Guid userId)
		{
            if (!await IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
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

            var agenda = await _eventRepository.GetAgendaByEventIdAsync(eventId);
			if (agenda == null)
			{
				_logger.LogWarning("No agenda found for event with id {EventId}", eventId);
            }
            return agenda;
        }

        public async Task<AgendaEntryDto> AddAgendaPointToEventAsync(
            Guid orgId, 
            Guid eventId, 
            AgendaEntryCreateDto agendaEntryDto,
            Guid userId)
        {
            if (!await IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                return null;
            }

            if (!await IsAuthenticated(userId, true, true, true, true))
            {
                _logger.LogWarning("User with id {UserId} is not authorized to add agenda points to events", userId);
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
    }
}
