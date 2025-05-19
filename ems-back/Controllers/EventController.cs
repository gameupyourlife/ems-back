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
using ems_back.Repo.Exceptions;

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

            if (!await _eventService.ExistsOrg(orgId)) {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
			{
				var events = await _eventService.GetAllEventsAsync(orgId, Guid.Parse(userId));
                _logger.LogInformation("Events found for organization with id {OrgId}", orgId);
				return Ok(events);
			}
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            if (!await _eventService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
            {
                

                var createdEvent = await _eventService.CreateEventAsync(orgId, eventDto, Guid.Parse(userId));
                if (createdEvent == null)
                {
                    _logger.LogWarning("Failed to create event");
                    return BadRequest("Failed to create event");
                }
                _logger.LogInformation("Event created successfully with id {EventId}", createdEvent.Id);
                return createdEvent;
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AlreadyExistsException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            if(!await _eventService.ExistsOrg(orgId)) {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
			{
                var eventEntity = await _eventService.GetEventAsync(orgId, eventId, Guid.Parse(userId));
				if (eventEntity == null)
				{
                    _logger.LogWarning("Event with id {EventId} not found", eventId);
                    return NotFound("Event not found");
                }
                _logger.LogInformation("Event with id {EventId} found", eventId);
                return Ok(eventEntity);
			}
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            if (!await _eventService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
            {
                var success = await _eventService.UpdateEventAsync(orgId, eventId, eventDto, Guid.Parse(userId));
                if (success == null)
                {
                    _logger.LogWarning("Failed to update event with id {EventId}", eventId);
                    return BadRequest("Failed to update event");
                }
                return Ok(success);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            if (!await _eventService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
            {
                var success = await _eventService.DeleteEventAsync(orgId, eventId, Guid.Parse(userId));

                if (!success) {
                    _logger.LogError("Failed to delete event with id {EventId}", eventId);
                    return NotFound("Event not found");
                }

                return Ok(success);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            if (!await _eventService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
			{
				var eventEntity = await _eventService.GetAllEventAttendeesAsync(orgId ,eventId, Guid.Parse(userId));
                if (eventEntity == null || !eventEntity.Any())
                {
                    _logger.LogWarning("No attendees found for event with id {EventId}", eventId);
                    return NotFound("No attendees found");
                }
                _logger.LogInformation("Attendees found for event with id {EventId}", eventId);
                return Ok(eventEntity);
			}
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
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

            try
            {
                var createdAttendee = await _eventService.AddAttendeeToEventAsync(orgId, eventId, attendee, Guid.Parse(userId));
                if (createdAttendee == null)
                {
                    _logger.LogWarning("Failed to add Attendee");
                    return BadRequest("Failed to add attendee");
                }

                _logger.LogInformation("Attendee added successfully with id {AttendeeId}", createdAttendee.UserId);
                return Ok(createdAttendee);
            }
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (AlreadyExistsException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NoCapacityException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting event attendees for event {EventId}", eventId);
                return StatusCode(500, "Internal server error");
            }
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

            if (!await _eventService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
            {
                var isDeleted = await _eventService.RemoveAttendeeFromEventAsync(orgId, eventId, attendeeId, Guid.Parse(userId));
                if (!isDeleted)
                {
                    _logger.LogWarning("Failed to remove attendee with id {AttendeeId}", attendeeId);
                    return NotFound("Attendee not found");
                }

                return Ok(isDeleted);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing attendee with id {AttendeeId} from event {EventId}", attendeeId, eventId);
                return StatusCode(500, "Internal server error");
            }
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

            if (!await _eventService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
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
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AlreadyExistsException ex)
            {
                return BadRequest("Event organizer already exists");
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

            if (!await _eventService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
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

            if (!await _eventService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
			{
				var eventEntity = await _eventService.GetAgendaAsync(orgId, eventId, Guid.Parse(userId));
				return eventEntity == null ? NotFound() : Ok(eventEntity);
			}
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
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

            if (!await _eventService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
            {
                var createdAgenda = await _eventService.AddAgendaEntryToEventAsync(orgId, eventId, createDto, Guid.Parse(userId));
                if (createdAgenda == null)
                {
                    _logger.LogWarning("Failed to add agenda");
                    return BadRequest("Failed to add agenda");
                }
                _logger.LogInformation("Agenda added successfully with id {AgendaId}", createdAgenda.Id);
                return Ok(createdAgenda);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding agenda to event");
                return StatusCode(500, "Internal server error");
            }
        }

        // Put: api/orgs/{orgId}/events/{eventId}/agenda/{agendaId}
        [HttpPut("{eventId}/agenda/{agendaId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<AgendaEntryDto>> UpdateAgendaInEvent(
            [FromRoute] Guid orgId,
            [FromRoute] Guid eventId, 
            [FromRoute] Guid agendaId,
            [FromBody] AgendaEntryCreateDto entry)
		{
            if (!await _eventService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            try
            {
                var updatedAgenda = await _eventService.UpdateAgendaEntryAsync(orgId, eventId, agendaId, entry, Guid.Parse(userId));
                if (updatedAgenda == null)
                {
                    _logger.LogWarning("Failed to update agenda with id {AgendaId}", agendaId);
                    return BadRequest("Failed to update agenda");
                }
                _logger.LogInformation("Agenda updated successfully with id {AgendaId}", updatedAgenda.Id);
                return Ok(updatedAgenda);
            }
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating agenda in event");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}/agenda/{agendaId}
        [HttpDelete("{eventId}/agenda/{agendaId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<EventInfoDto>> RemoveAgendaFromEvent(
            [FromRoute] Guid orgId,
            [FromRoute] Guid eventId, 
            [FromRoute] Guid agendaId)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            if (!await _eventService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
            {
                var deletedAgenda = await _eventService.DeleteAgendaEntryAsync(orgId, eventId, agendaId, Guid.Parse(userId));
                if (!deletedAgenda)
                {
                    _logger.LogWarning("Failed to delete agenda with id {AgendaId}", agendaId);
                    return BadRequest("Failed to delete agenda");
                }
                _logger.LogInformation("Agenda deleted successfully");
                return Ok(deletedAgenda);
            }
            catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting agenda from event");
                return StatusCode(500, "Internal server error");

            }
        }
	}
}