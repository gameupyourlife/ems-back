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
			try
			{
				var events = await _eventService.GetAllEventsAsync(orgId);
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
        //[Authorize(Roles = "Organizer,Admin,EventOrganizer")]
        public async Task<ActionResult<EventInfoDto>> CreateEvent(
			[FromBody] EventCreateDto eventDto, 
			[FromRoute] Guid orgId)
        {
            try
            {
                var createdEvent = await _eventService.CreateEventAsync(orgId, eventDto);
                if (createdEvent == null)
                {
                    _logger.LogWarning("Failed to create event");
                    return BadRequest("Failed to create event");
                }
				_logger.LogInformation("Event created successfully with id {EventId}", createdEvent.Id);
                return createdEvent;
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
				var eventEntity = await _eventService.GetEventAsync(orgId, eventId);
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

        // PUT: api/orgs/{orgId}/events/{eventId}
        [HttpPut("{eventId}")]
        public async Task<ActionResult<EventInfoDto>> UpdateEvent(
            [FromRoute] Guid orgId, 
            [FromRoute] Guid eventId, 
            [FromBody] EventInfoDto eventDto)
        {
            // To Do: Check

            try
            {
                if (eventId != eventDto.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var updatedEvent = await _eventService.UpdateEventAsync(orgId, eventId, eventDto);
                if (updatedEvent == null)
                {
                    _logger.LogWarning("Failed to update event with id {EventId}", eventId);
                    return NotFound("Event not found");
                }

                return Ok(updatedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event with id {EventId}", eventId);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteEvent(
            [FromRoute] Guid orgId, 
            [FromRoute] Guid eventId)
        {

            // To Do: Check

            try
            {
                var success = await _eventService.DeleteEventAsync(orgId, eventId);
                return success ? NoContent() : NotFound();
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
        public async Task<ActionResult<EventInfoDto>> AddAttendeeToEvent(
			[FromRoute] Guid orgId ,
			[FromRoute] Guid eventId)
		{
			var attendeeList = await _eventService.GetAllEventAttendeesAsync(orgId, eventId);
            if (attendeeList == null || !attendeeList.Any())
            {
                _logger.LogWarning("No attendees found for event with id {EventId}", eventId);
                return NotFound("No attendees found");
            }
            _logger.LogInformation("Attendees found for event with id {EventId}", eventId);
            return Ok(attendeeList);
        }


        // DELETE: api/orgs/{orgId}/events/{eventId}/attendees/{attendeeId}
        [HttpDelete("{eventId}/attendees/{userId}")]
        public async Task<ActionResult<EventInfoDto>> RemoveAttendeeFromEvent(Guid eventId, Guid attendeeId)
		{
			throw new NotImplementedException("This method is not implemented yet.");
        }

        // GET: api/orgs/{orgId}/events/{eventId}/agenda
        [HttpGet("{eventId}/agenda")]
		public async Task<ActionResult<List<AgendaEntry>>> GetAgendaByEvent(
            [FromRoute] Guid orgId, 
            [FromRoute] Guid eventId)
		{
			try
			{
				var eventEntity = await _eventService.GetAgendaAsync(orgId, eventId);
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
        public async Task<ActionResult<EventInfoDto>> AddAgendaToEvent(Guid eventId, Guid agendaId)
		{
			throw new NotImplementedException("This method is not implemented yet.");
        }

        // Put: api/orgs/{orgId}/events/{eventId}/agenda/{agendaId}
        [HttpPut("{eventId}/agenda/{agendaId}")]
        public async Task<ActionResult<EventInfoDto>> UpdateAgendaInEvent(Guid eventId, Guid agendaId)
		{
			throw new NotImplementedException("This method is not implemented yet.");
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}/agenda/{agendaId}
        [HttpDelete("{eventId}/agenda/{agendaId}")]
        public async Task<ActionResult<EventInfoDto>> RemoveAgendaFromEvent(Guid eventId, Guid agendaId)
		{
            throw new NotImplementedException("This method is not implemented yet.");
        }

        // GET: api/orgs/{orgId}/events/{eventId}/files
        [HttpGet("{eventId}/files")]
        public async Task<ActionResult<List<FileDto>>> GetFilesfromEvent(
            [FromRoute] Guid orgId, 
            [FromRoute] Guid eventId)
		{
			var fileList = await _eventService.GetFilesFromEventAsync(orgId, eventId);
            return fileList == null ? NotFound() : Ok(fileList);
        }

        // POST: api/orgs/{orgId}/events/{eventId}/files
        [HttpPost("{eventId}/files")]
        public async Task<ActionResult<EventInfoDto>> AddFileToEvent(Guid eventId, Guid fileId)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        // PUT: api/orgs/{orgId}/events/{eventId}/files/{fileId}
        [HttpPut("{eventId}/files/{fileId}")]
        public async Task<ActionResult<EventInfoDto>> UpdateFileInEvent(Guid eventId, Guid fileId)
		{
			throw new NotImplementedException("This method is not implemented yet.");
        }
        // DELETE: api/orgs/{orgId}/events/{eventId}/files/{fileId}
        [HttpDelete("{eventId}/files/{fileId}")]
        public async Task<ActionResult<EventInfoDto>> RemoveFileFromEvent(Guid eventId, Guid fileId)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }
	}
}