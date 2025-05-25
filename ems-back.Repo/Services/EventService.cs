using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.Agenda;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Exceptions;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services
{
	public class EventService : IEventService
	{
		private readonly IEventRepository _eventRepository;
        private readonly IUserService _userService;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ILogger<EventService> _logger;

        public EventService(
			IEventRepository eventRepository,
			IUserService userService,
            IOrganizationRepository organizationRepository,
            ILogger<EventService> logger)
		{
			_eventRepository = eventRepository;
            _userService = userService;
            _organizationRepository = organizationRepository;
            _logger = logger;
        }

        public async Task<bool> ExistsOrg(Guid orgId)
        {
            var org = await _organizationRepository.GetOrganizationByIdAsync(orgId);
            if (org == null)
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return false;
            }

            return true;
        }

        public async Task<IEnumerable<EventOverviewDto>> GetAllEventsAsync(Guid orgId, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            var events = await _eventRepository.GetAllEventsAsync(orgId);

            if (events == null || !events.Any())
            {
                _logger.LogWarning("No events found for organization with id {OrgId}", orgId);
                throw new NotFoundException("No events found");
            }
            else
            {
                var updatedEvents = new List<EventOverviewDto>();
                foreach (var ev in events)
                {
                    var isAttending = await _eventRepository.GetEventAttendeeByIdAsync(ev.Id, userId) != null;
                    ev.isAttending = isAttending;
                    updatedEvents.Add(ev);
                }
                return updatedEvents;
            }
        }

        public async Task<EventInfoDto> CreateEventAsync(Guid orgId, EventCreateDto eventDto, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            if (eventDto.Start < DateTime.UtcNow)
            {
                _logger.LogWarning("Event start date {StartDate} is in the past", eventDto.Start);
                throw new InvalidOperationException("Event start date cannot be in the past");
            }

            if (eventDto.Start > eventDto.End)
            {
                _logger.LogWarning("Event start date {StartDate} is after the end date {EndDate}", eventDto.Start, eventDto.End);
                throw new InvalidOperationException("Event start date cannot be after the end date");
            }

            if (eventDto.Capacity <= 0)
            {
                _logger.LogWarning("Event capacity {Capacity} must be greater than zero", eventDto.Capacity);
                throw new InvalidOperationException("Event capacity must be greater than zero");
            }

            var user = await _userService.GetUserByIdAsync(userId);

            var attends = await _eventRepository.GetEventAttendeeByIdAsync(orgId, userId) != null;

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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedBy = userId,
                CreatorName = user.FullName,
                AttendeeCount = 0,
                isAttending = attends,
            };

            // Check if the event already exists

            var existingEvent = await _eventRepository.GetEventByTitleAndDateAsync(eventInfo.Title, eventInfo.Start, orgId);

            if (existingEvent != null)
            {
                _logger.LogWarning("Event with title {Title} and start date {StartDate} already exists", eventInfo.Title, eventInfo.Start);
                throw new AlreadyExistsException("Event with the same title and start date already exists");
            }
            
            var eventId = await _eventRepository.CreateEventAsync(eventInfo);
            eventInfo.Id = eventId;
            return eventInfo;
        }

        public async Task<EventInfoDto> GetEventAsync(Guid orgId, Guid eventid, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            var eventEntity = await _eventRepository.GetEventByIdAsync(orgId, eventid);
            if (eventEntity == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventid);
                throw new NotFoundException("Event not found");
            }

            eventEntity.isAttending = await _eventRepository.GetEventAttendeeByIdAsync(eventEntity.Id, userId) != null;

            return eventEntity;
        }

        public async Task<EventInfoDto> UpdateEventAsync(
            Guid orgId, 
            Guid eventId, 
            EventUpdateDto eventDto,
            Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            if (eventDto.Start < DateTime.UtcNow)
            {
                _logger.LogWarning("Event start date {StartDate} is in the past", eventDto.Start);
                throw new InvalidOperationException("Event start date cannot be in the past");
            }

            if (eventDto.Start > eventDto.End)
            {
                _logger.LogWarning("Event start date {StartDate} is after the end date {EndDate}", eventDto.Start, eventDto.End);
                throw new InvalidOperationException("Event start date cannot be after the end date");
            }

            if (eventDto.Capacity <= 0)
            {
                _logger.LogWarning("Event capacity {Capacity} must be greater than zero", eventDto.Capacity);
                throw new InvalidOperationException("Event capacity must be greater than zero");
            }

            return await _eventRepository.UpdateEventAsync(orgId, eventId, eventDto, userId);
        }

        public async Task<bool> DeleteEventAsync(Guid orgId, Guid eventId, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            return await _eventRepository.DeleteEventAsync(orgId, eventId);
        }

        public async Task<IEnumerable<EventAttendeeDto>> GetAllEventAttendeesAsync(Guid orgId, Guid eventId, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", eventId, orgId);
                throw new MismatchException("User is not member of org");
            }

            var attendeeList = await _eventRepository.GetAllEventAttendeesAsync(orgId, eventId);
            if (attendeeList == null)
            {
                _logger.LogWarning("No attendees found for event with id {EventId}", eventId);
                throw new NotFoundException("No attendees found");
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
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            if (await _eventRepository.GetEventAttendeeByIdAsync(eventId, attendeeDto.UserId) != null)
            {
                _logger.LogWarning("User with id {UserId} is already registered for event with id {EventId}", attendeeDto.UserId, eventId);
                throw new AlreadyExistsException("User is already registered for this event");
            }

            var eventInfo = await _eventRepository.GetEventByIdAsync(orgId, eventId);

            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                throw new NotFoundException("Event not found"); 
            }

            if (eventInfo.AttendeeCount >= eventInfo.Capacity)
            {
                _logger.LogWarning("Event with id {EventId} is full", eventId);
                throw new NoCapacityException("Event is full");
            }

            var attendee = new EventAttendee
            {
                UserId = attendeeDto.UserId,
                EventId = eventId,
                RegisteredAt = DateTime.UtcNow,
                Status = AttendeeStatus.Pending
            };

            try
            {
                var isCreated = await _eventRepository.AddAttendeeToEventAsync(attendee);

                if (!isCreated)
                {
                    _logger.LogWarning("Failed to add attendee to event with id {EventId}", eventId);
                    throw new DbUpdateException("Failed to add attendee to event");
                }
            }

            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error adding attendee to event with id {EventId}", eventId);
                throw new DbUpdateException("Error adding attendee to event");
            }

            var user = await _userService.GetUserByIdAsync(attendeeDto.UserId);
            var attendeeInfo = new EventAttendeeDto
            {
                UserId = attendee.UserId,
                UserEmail = user.Email,
                UserName = user.FullName,
                Status = attendee.Status,
                RegisteredAt = attendee.RegisteredAt
            };

            _logger.LogInformation("Attendee added to event with id {EventId}", eventId);
            return attendeeInfo;
        }

        public async Task<bool> RemoveAttendeeFromEventAsync(Guid orgId, Guid eventId, Guid attendeeId, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            var eventInfo = await _eventRepository.GetEventByIdAsync(orgId, eventId);
            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                throw new NotFoundException("Event not found");
            }

            return await _eventRepository.RemoveAttendeeFromEventAsync(eventId, attendeeId);
        }

        public async Task<bool> AddEventOrganizerAsync(Guid orgId, Guid eventId, Guid organizerId, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            if (!await _userService.IsUserInOrgOrAdmin(orgId, organizerId))
            {
                _logger.LogWarning("User with id {OrganizerId} is not a member of organization with id {OrgId}", organizerId, orgId);
                throw new MismatchException("User is not member of org");
            }

            var eventInfo = await _eventRepository.GetEventByIdAsync(orgId, eventId);
            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                throw new NotFoundException("Event not found");
            }

            var organizer = await _eventRepository.GetEventOrganizerAsync(eventId, organizerId);
            if (organizer != null)
            {
                _logger.LogWarning("Event organizer with id {OrganizerId} already exists for event with id {EventId}", organizerId, eventId);
                throw new AlreadyExistsException("Event organizer already exists for this event");
            }

            var isAdded = await _eventRepository.AddEventOrganizerAsync(orgId, eventId, organizerId);
            if (!isAdded)
            {
                _logger.LogWarning("Event organizer with id {OrganizerId} could not be added to event with id {EventId}", organizerId, eventId);
                throw new DbUpdateException("Event organizer could not be added");
            }

            var roleUpdate = new UserUpdateRoleDto
            {
                userId = organizerId,
                OrganizationId = orgId,
                newRole = UserRole.EventOrganizer
            };

            var isRoleUpdated = await _userService.UpdateUserRoleAsync(organizerId, roleUpdate);
            if (!isRoleUpdated)
            {
                _logger.LogWarning("Event organizer role could not be updated for user with id {OrganizerId}", organizerId);
                await RemoveEventOrganizerAsync(orgId, eventId, organizerId, userId);
                throw new MismatchException("Event organizer role could not be updated");
            }

            return isRoleUpdated;
        }

        public async Task<bool> RemoveEventOrganizerAsync(Guid orgId, Guid eventId, Guid organizerId, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            var isRemoved = await _eventRepository.RemoveEventOrganizerAsync(orgId, eventId, organizerId);
            if (!isRemoved)
            {
                _logger.LogWarning("Organizer could not be removed");
                throw new MismatchException("Organizer could not be removed");
            }

            var roleUpdate = new UserUpdateRoleDto
            {
                userId = organizerId,
                OrganizationId = orgId,
                newRole = UserRole.EventOrganizer
            };

            var isroleUpdated = await _userService.UpdateUserRoleAsync(organizerId, roleUpdate);
            if (!isroleUpdated)
            {
                _logger.LogWarning("Event organizer role could not be updated");
                await AddEventOrganizerAsync(orgId, eventId, organizerId, userId);
                throw new MismatchException("Event organizer role could not be updated");
            }

            return isroleUpdated;
        }

        public async Task<IEnumerable<AgendaEntryDto>> GetAgendaAsync(Guid orgId, Guid eventId, Guid userId)
		{
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            var eventInfo = await _eventRepository.GetEventByIdAsync(orgId, eventId);
            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                throw new NotFoundException("Event not found");
            }

            if (eventInfo.OrganizationId != orgId)
            {
                _logger.LogWarning("Event with id {EventId} does not belong to organization with id {OrgId}", eventId, orgId);
                throw new MismatchException("Given event is not in given organization");
            }

            var agenda = await _eventRepository.GetAgendaByEventIdAsync(eventId);
			if (agenda == null)
			{
				_logger.LogWarning("No agenda found for event with id {EventId}", eventId);
                throw new NotFoundException("No agenda found");
            }
            return agenda;
        }

        public async Task<AgendaEntryDto> AddAgendaEntryToEventAsync(
            Guid orgId, 
            Guid eventId, 
            AgendaEntryCreateDto agendaEntryDto,
            Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            var eventInfo = await _eventRepository.GetEventByIdAsync(orgId, eventId);
            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                throw new NotFoundException("Event not found");
            }

            var agendaEntry = new AgendaEntryDto
            {
                Id = Guid.NewGuid(),
                Title = agendaEntryDto.Title,
                Description = agendaEntryDto.Description,
                Start = agendaEntryDto.Start,
                End = agendaEntryDto.End,
                EventId = eventId,
            };

            var agendaId = await _eventRepository.AddAgendaEntryToEventAsync(agendaEntry);

            agendaEntry.Id = agendaId;
            _logger.LogInformation("Agenda point added to event with id {EventId}", eventId);
            return agendaEntry;
        }

        public async Task<AgendaEntryDto> UpdateAgendaEntryAsync(
            Guid orgId, 
            Guid eventId, 
            Guid agendaId, 
            AgendaEntryCreateDto agendaEntryDto,
            Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            var eventInfo = await _eventRepository.GetEventByIdAsync(orgId, eventId);
            if (eventInfo == null) {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                throw new NotFoundException("Event not found");
            }

            if (await _eventRepository.GetAgendaEntryByIdAsync(agendaId) == null)
            {
                _logger.LogWarning("Agenda entry with id {AgendaId} not found", agendaId);
                throw new NotFoundException("Agenda entry not found");
            }

            var agendaEntry = new AgendaEntryDto
            {
                Id = agendaId,
                Title = agendaEntryDto.Title,
                Description = agendaEntryDto.Description,
                Start = agendaEntryDto.Start,
                End = agendaEntryDto.End,
                EventId = eventId
            };

            if (!await _eventRepository.UpdateAgendaEntryAsync(agendaId, eventId, agendaEntry))
            {
                _logger.LogWarning("Failed to update agenda entry with id {AgendaId} for event with id {EventId}", agendaId, eventId);
                throw new NotFoundException("Failed to update agenda entry");
            }
            
            _logger.LogInformation("Agenda point with id {AgendaId} updated for event with id {EventId}", agendaId, eventId);
            return agendaEntry;
        }

        public async Task<bool> DeleteAgendaEntryAsync(Guid orgId, Guid eventId, Guid agendaId, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            var eventInfo = await _eventRepository.GetEventByIdAsync(orgId, eventId);
            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                throw new NotFoundException("Event not found");
            }

            if (eventInfo.OrganizationId != orgId)
            {
                _logger.LogWarning("Event with id {EventId} does not belong to organization with id {OrgId}", eventId, orgId);
                throw new MismatchException("Given event is not in given organization");
            }

            var agendaEntry = await _eventRepository.GetAgendaEntryByIdAsync(agendaId);
            if (agendaEntry == null)
            {
                _logger.LogWarning("Agenda entry with id {AgendaId} not found", agendaId);
                throw new NotFoundException("Agenda entry not found");
            }

            var isDeleted = await _eventRepository.DeleteAgendaEntryAsync(agendaId);
            if (!isDeleted)
            {
                _logger.LogWarning("Failed to delete agenda entry with id {AgendaId} for event with id {EventId}", agendaId, eventId);
                return false;
            }

            _logger.LogInformation("Agenda point with id {AgendaId} deleted for event with id {EventId}", agendaId, eventId);
            return true;
        }

        public async Task<IEnumerable<EventOverviewDto>> GetAllEventsByCreatorAsync(Guid orgId, Guid creatorId, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not a member of organization with id {OrgId}", userId, orgId);
                throw new MismatchException("User is not member of org");
            }

            var events = await _eventRepository.GetAllEventsByCreatorAsync(orgId, creatorId);

            if (events == null || !events.Any())
            {
                _logger.LogWarning("No events found for organization with id {OrgId}", orgId);
                throw new NotFoundException("No events found");
            }
            else
            {
                return events;
            }
        }
    }
}
