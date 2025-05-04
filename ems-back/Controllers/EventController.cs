using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Authorization;

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
		public async Task<ActionResult<IEnumerable<EventOverviewDto>>> GetAllEvents(Guid orgId)
		{
			try
			{
				var events = await _eventService.GetAllEventsAsync(orgId);
				return Ok(events);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting all events");
				return StatusCode(500, "Internal server error");
			}
		}

        // GET: api/orgs/{orgId}/events/{eventId}
        [HttpGet("{eventId}")]
		public async Task<ActionResult<EventInfoDTO>> GetEvent(Guid orgId, Guid eventId)
		{
			try
			{
				var eventEntity = await _eventService.GetEventByIdAsync(orgId, eventId);
				return eventEntity == null ? NotFound() : Ok(eventEntity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting event with id {EventId}", eventId);
				return StatusCode(500, "Internal server error");
			}
		}

        // GET: api/orgs/{orgId}/events/{eventId}/attendees
        [HttpGet("{eventId}/attendees")]
		public async Task<ActionResult<EventInfoDTO>> GetEventWithAttendees(Guid id)
		{
			try
			{
				var eventEntity = await _eventService.GetEventWithAttendeesAsync(id);
				return eventEntity == null ? NotFound() : Ok(eventEntity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting event attendees for event {EventId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

        // POST: api/orgs/{orgId}/events/{eventId}/attendees
        [HttpPost("{eventId}/attendees")]
        public async Task<ActionResult<EventInfoDTO>> AddAttendeeToEvent(Guid eventId, Guid attendeeId)
		{
			throw new NotImplementedException("This method is not implemented yet.");
		}

        // DELETE: api/orgs/{orgId}/events/{eventId}/attendees/{attendeeId}
        [HttpDelete("{eventId}/attendees/{userId}")]
        public async Task<ActionResult<EventInfoDTO>> RemoveAttendeeFromEvent(Guid eventId, Guid attendeeId)
		{
			throw new NotImplementedException("This method is not implemented yet.");
        }

        // GET: api/orgs/{orgId}/events/{eventId}/agenda
        [HttpGet("{eventId}/agenda")]
		public async Task<ActionResult<EventInfoDTO>> GetEventWithAgenda(Guid id)
		{
			try
			{
				var eventEntity = await _eventService.GetEventWithAgendaAsync(id);
				return eventEntity == null ? NotFound() : Ok(eventEntity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting event agenda for event {EventId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

        // POST: api/orgs/{orgId}/events/{eventId}/agenda
        [HttpPost("{eventId}/agenda")]
        public async Task<ActionResult<EventInfoDTO>> AddAgendaToEvent(Guid eventId, Guid agendaId)
		{
			throw new NotImplementedException("This method is not implemented yet.");
        }

        // PUt: api/orgs/{orgId}/events/{eventId}/agenda/{agendaId}
        [HttpPut("{eventId}/agenda/{agendaId}")]
        public async Task<ActionResult<EventInfoDTO>> UpdateAgendaInEvent(Guid eventId, Guid agendaId)
		{
			throw new NotImplementedException("This method is not implemented yet.");
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}/agenda/{agendaId}
        [HttpDelete("{eventId}/agenda/{agendaId}")]
        public async Task<ActionResult<EventInfoDTO>> RemoveAgendaFromEvent(Guid eventId, Guid agendaId)
		{
            throw new NotImplementedException("This method is not implemented yet.");
        }

        // GET: api/orgs/{orgId}/events/{eventId}/files
        [HttpGet("{eventId}/files")]
        public async Task<ActionResult<EventInfoDTO>> GetFilesfromEvent(Guid id)
		{
			throw new NotImplementedException("This method is not implemented yet.");
        }

        // POST: api/orgs/{orgId}/events/{eventId}/files
        [HttpPost("{eventId}/files")]
        public async Task<ActionResult<EventInfoDTO>> AddFileToEvent(Guid eventId, Guid fileId)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        // PUT: api/orgs/{orgId}/events/{eventId}/files/{fileId}
        [HttpPut("{eventId}/files/{fileId}")]
        public async Task<ActionResult<EventInfoDTO>> UpdateFileInEvent(Guid eventId, Guid fileId)
		{
			throw new NotImplementedException("This method is not implemented yet.");
        }
        // DELETE: api/orgs/{orgId}/events/{eventId}/files/{fileId}
        [HttpDelete("{eventId}/files/{fileId}")]
        public async Task<ActionResult<EventInfoDTO>> RemoveFileFromEvent(Guid eventId, Guid fileId)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        [HttpPost]
		[Authorize(Roles = "Organizer,Admin")]
		public async Task<ActionResult<EventInfoDTO>> CreateEvent([FromBody] EventCreateDto eventDto)
		{
			try
			{
				var createdEvent = await _eventService.CreateEventAsync(eventDto);
				return CreatedAtAction(
					nameof(GetEvent),
					new { id = createdEvent.Metadata.Id },
					createdEvent);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating event");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPut("{eventId}")]
		public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] EventInfoDTO eventDto)
		{
			try
			{
				if (id != eventDto.Id)
				{
					return BadRequest("ID mismatch");
				}

				var success = await _eventService.UpdateEventAsync(id, eventDto);
				return success ? NoContent() : NotFound();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating event with id {EventId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpDelete("{eventId}")]
		public async Task<IActionResult> DeleteEvent(Guid id)
		{
			try
			{
				var success = await _eventService.DeleteEventAsync(id);
				return success ? NoContent() : NotFound();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting event with id {EventId}", id);
				return StatusCode(500, "Internal server error");
			}
		}
	}
}