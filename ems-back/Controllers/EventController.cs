using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Authorization;
using ems_back.Repo.Models;
using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.Agenda;
using System.Security.Claims;
using System.Diagnostics.Eventing.Reader;

namespace ems_back.Controllers
{
    [Route("api/orgs/{orgId}/events")]
	[ApiController]
	public class EventsController : ControllerBase
	{
		private readonly IEventService _eventService;
		private readonly ILogger<EventsController> _logger;

		public EventsController(
			IEventService eventService,
			ILogger<EventsController> logger)
		{
			_eventService = eventService;
			_logger = logger;
		}

        // GET: api/orgs/{orgId}/events
        [HttpGet]
		public async Task<ActionResult<IEnumerable<EventOverviewDto>>> GetAllEvents([FromRoute] Guid orgId)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            try
			{
				var events = await _eventService.GetAllEventsAsync(orgId, Guid.Parse(userId));
                if (events == null || !events.Any())
                {
                    _logger.LogWarning("No events found for organization with id {OrgId}", orgId);
                    return NotFound("No events found");
                }
                    _logger.LogInformation("Events found for organization with id {OrgId}", orgId);
					return Ok(events);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting all events");
				return StatusCode(500, "Internal server error");
			}
		}

        // POST: api/orgs/{orgId}/events
        [HttpPost]  
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}")]
        public async Task<ActionResult<EventInfoDto>> CreateEvent(
			[FromRoute] Guid orgId,
            [FromBody] EventCreateDto eventDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in claims");
                    return BadRequest("User ID not found");
                }

                var createdEvent = await _eventService.CreateEventAsync(orgId, eventDto, Guid.Parse(userId));
                if (createdEvent == null)
                {
                    _logger.LogWarning("Failed to create event");
                    return BadRequest("Failed to create event");
                }
                _logger.LogInformation("Event created successfully with id {EventId}", createdEvent.Id);
                return createdEvent;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access while creating event");
                return Unauthorized("User is not authorized to create events");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                return StatusCode(500, "Internal server error");
            }
            
        }

        // GET: api/orgs/{orgId}/events/{eventId}
        [HttpGet("{eventId}")]
		public async Task<ActionResult<EventInfoDto>> GetEventById(
			[FromRoute] Guid orgId,
			[FromRoute] Guid eventId)
		{
            try
			{
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in claims");
                    return BadRequest("User ID not found");
                }

                var eventEntity = await _eventService.GetEventAsync(orgId, eventId, Guid.Parse(userId));
				if (eventEntity == null)
				{
                    _logger.LogWarning("Event with id {EventId} not found", eventId);
                    return NotFound("Event not found");
                }
                _logger.LogInformation("Event with id {EventId} found", eventId);
                return Ok(eventEntity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting event with id {EventId}", eventId);
				return StatusCode(500, "Internal server error");
			}
		}

        [HttpPut("{eventId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}")]
        public async Task<ActionResult<EventInfoDto>> UpdateEvent(
            [FromRoute] Guid orgId, 
            [FromRoute] Guid eventId, 
            [FromBody] EventUpdateDto eventDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in claims");
                    return BadRequest("User ID not found");
                }

                var success = await _eventService.UpdateEventAsync(orgId, eventId, eventDto, Guid.Parse(userId));

                if (success == null)
                {
                    _logger.LogWarning("Failed to update event with id {EventId}", eventId);
                    return BadRequest("Failed to update event");
                }

                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event with id {EventId}", eventId);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}
        [HttpDelete("{eventId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}")]
        public async Task<ActionResult<bool>> DeleteEvent(
            [FromRoute] Guid orgId, 
            [FromRoute] Guid eventId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in claims");
                    return BadRequest("User ID not found");
                }

                var success = await _eventService.DeleteEventAsync(orgId, eventId, Guid.Parse(userId));

                if (!success) {
                    _logger.LogError("Failed to delete event with id {EventId}", eventId);
                    return NotFound("Event not found");
                }

                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event with id {EventId}", eventId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/orgs/{orgId}/events/{eventId}/attendees
        [HttpGet("{eventId}/attendees")]
		public async Task<ActionResult<List<EventAttendeeDto>>> GetAllAttendeesFromEvent(
			[FromRoute] Guid orgId ,
			[FromRoute] Guid eventId)
		{
			try
			{
				var eventEntity = await _eventService.GetAllEventAttendeesAsync(orgId ,eventId);
                if (eventEntity == null || !eventEntity.Any())
                {
                    _logger.LogWarning("No attendees found for event with id {EventId}", eventId);
                    return NotFound("No attendees found");
                }
                _logger.LogInformation("Attendees found for event with id {EventId}", eventId);
                return Ok(eventEntity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting event attendees for event {EventId}", eventId);
				return StatusCode(500, "Internal server error");
			}
		}

        // POST: api/orgs/{orgId}/events/{eventId}/attendees
        [HttpPost("{eventId}/attendees")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}")]
        public async Task<ActionResult<EventAttendeeDto>> AddAttendeeToEvent(
			[FromRoute] Guid orgId ,
			[FromRoute] Guid eventId,
            [FromBody] EventAttendeeCreateDto attendee)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            var createdAttendee = await _eventService.AddAttendeeToEventAsync(orgId, eventId, attendee, Guid.Parse(userId));
            if (createdAttendee == null)
            {
                _logger.LogWarning("Failed to add Attendee");
                return BadRequest("Failed to add attendee");
            }

            _logger.LogInformation("Attendee added successfully with id {AttendeeId}", createdAttendee.UserId);
            return Ok(createdAttendee);
        }


        // DELETE: api/orgs/{orgId}/events/{eventId}/attendees/{userId}
        [HttpDelete("{eventId}/attendees/{attendeeId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}")]
        public async Task<ActionResult<bool>> RemoveAttendeeFromEvent(
            [FromRoute] Guid orgId,
            [FromRoute] Guid eventId, 
            [FromRoute] Guid attendeeId)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            var isDeleted = await _eventService.RemoveAttendeeFromEventAsync(orgId, eventId, attendeeId, Guid.Parse(userId));
            if (!isDeleted)
            {
                _logger.LogWarning("Failed to remove attendee with id {AttendeeId}", attendeeId);
                return NotFound("Attendee not found");
            }

            return Ok(isDeleted);
        }

        // POST: api/orgs/{orgId}/events/{eventId}/eventOrganizer/{organizerId}
        [HttpPost("{eventId}/eventOrganizer/{organizerId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}")]

        public async Task<ActionResult<bool>> AddEventOrganizer(
            [FromRoute] Guid orgId,
            [FromRoute] Guid eventId,
            [FromRoute] Guid organizerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            try
            {
                var isCreated = await _eventService.AddEventOrganizerAsync(orgId, eventId, organizerId, Guid.Parse(userId));
                if (!isCreated)
                {
                    _logger.LogWarning("Failed to add event organizer with id {OrganizerId}", organizerId);
                    return BadRequest("Failed to add event organizer");
                }

                return Ok(isCreated);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding event organizer");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}/eventOrganizer/{organizerId}
        [HttpDelete("{eventId}/eventOrganizer/{organizerId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}")]

        public async Task<ActionResult<bool>> RemoveEventOrganizer(
            [FromRoute] Guid orgId,
            [FromRoute] Guid eventId,
            [FromRoute] Guid organizerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }
            try
            {
                var isDeleted = await _eventService.RemoveEventOrganizerAsync(orgId, eventId, organizerId, Guid.Parse(userId));
                if (!isDeleted)
                {
                    _logger.LogWarning("Failed to remove event organizer with id {OrganizerId}", organizerId);
                    return NotFound("Event organizer not found");
                }
                return Ok(isDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing event organizer");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/orgs/{orgId}/events/{eventId}/agenda
        [HttpGet("{eventId}/agenda")]
		public async Task<ActionResult<List<AgendaEntry>>> GetAgendaByEvent(
            [FromRoute] Guid orgId, 
            [FromRoute] Guid eventId)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            try
			{
				var eventEntity = await _eventService.GetAgendaAsync(orgId, eventId, Guid.Parse(userId));
				return eventEntity == null ? NotFound() : Ok(eventEntity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting event agenda for event {EventId}", eventId);
				return StatusCode(500, "Internal server error");
			}
		}

        // POST: api/orgs/{orgId}/events/{eventId}/agenda
        [HttpPost("{eventId}/agenda")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}")]
        public async Task<ActionResult<EventInfoDto>> AddAgendaToEvent(
            [FromRoute] Guid orgId, 
            [FromRoute] Guid eventId,
            [FromBody] AgendaEntryCreateDto createDto)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            var createdAgenda = await _eventService.AddAgendaPointToEventAsync(orgId, eventId, createDto, Guid.Parse(userId));
            if (createdAgenda == null)
            {
                _logger.LogWarning("Failed to add agenda");
                return BadRequest("Failed to add agenda");
            }
            _logger.LogInformation("Agenda added successfully with id {AgendaId}", createdAgenda.Id);
            return Ok(createdAgenda);
        }

        // Put: api/orgs/{orgId}/events/{eventId}/agenda/{agendaId}
        [HttpPut("{eventId}/agenda/{agendaId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}")]
        public async Task<ActionResult<EventInfoDto>> UpdateAgendaInEvent(Guid eventId, Guid agendaId)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            throw new NotImplementedException("This method is not implemented yet.");
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}/agenda/{agendaId}
        [HttpDelete("{eventId}/agenda/{agendaId}")]
        public async Task<ActionResult<EventInfoDto>> RemoveAgendaFromEvent(Guid eventId, Guid agendaId)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            throw new NotImplementedException("This method is not implemented yet.");
        }
	}
}