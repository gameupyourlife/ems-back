using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Authorization;
using ems_back.Repo.Services.Interfaces;

namespace ems_back.Controllers
{
    [Route("api/orgs/{orgId}/[controller]")]
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

		[HttpGet]
		public async Task<ActionResult<IEnumerable<EventBasicDto>>> GetAllEvents()
		{
			try
			{
				var events = await _eventService.GetAllEventsAsync();
				return Ok(events);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting all events");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("upcoming")]
		public async Task<ActionResult<IEnumerable<EventBasicDto>>> GetUpcomingEvents([FromQuery] int days = 30)
		{
			try
			{
				var events = await _eventService.GetUpcomingEventsAsync(days);
				return Ok(events);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting upcoming events");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<EventBasicDetailedDto>> GetEvent(Guid id)
		{
			try
			{
				var eventEntity = await _eventService.GetEventByIdAsync(id);
				return eventEntity == null ? NotFound() : Ok(eventEntity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting event with id {EventId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("{id}/attendees")]
		public async Task<ActionResult<EventBasicDetailedDto>> GetEventWithAttendees(Guid id)
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

		[HttpGet("{id}/agenda")]
		public async Task<ActionResult<EventBasicDetailedDto>> GetEventWithAgenda(Guid id)
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

		[HttpGet("{id}/details")]
		public async Task<ActionResult<EventBasicDetailedDto>> GetEventWithAllDetails(Guid id)
		{
			try
			{
				var eventEntity = await _eventService.GetEventWithAllDetailsAsync(id);
				return eventEntity == null ? NotFound() : Ok(eventEntity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting event details for event {EventId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("organization/{organizationId}")]
		public async Task<ActionResult<IEnumerable<EventBasicDto>>> GetEventsByOrganization(Guid organizationId)
		{
			try
			{
				var events = await _eventService.GetEventsByOrganizationAsync(organizationId);
				return Ok(events);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting events for organization {OrganizationId}", organizationId);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("creator/{userId}")]
		public async Task<ActionResult<IEnumerable<EventBasicDto>>> GetEventsByCreator(Guid userId)
		{
			try
			{
				var events = await _eventService.GetEventsByCreatorAsync(userId);
				return Ok(events);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting events for creator {UserId}", userId);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("category/{category}")]
		public async Task<ActionResult<IEnumerable<EventBasicDto>>> GetEventsByCategory(EventCategory category)
		{
			try
			{
				var events = await _eventService.GetEventsByCategoryAsync(category);
				return Ok(events);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting events for category {Category}", category);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("date-range")]
		public async Task<ActionResult<IEnumerable<EventBasicDto>>> GetEventsByDateRange(
			[FromQuery] DateTime start, [FromQuery] DateTime end)
		{
			try
			{
				var events = await _eventService.GetEventsByDateRangeAsync(start, end);
				return Ok(events);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting events between {StartDate} and {EndDate}", start, end);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPost]
		[Authorize(Roles = "Organizer,Admin")]
		public async Task<ActionResult<EventBasicDetailedDto>> CreateEvent([FromBody] EventCreateDto eventDto)
		{
			try
			{
				var createdEvent = await _eventService.CreateEventAsync(eventDto);
				return CreatedAtAction(
					nameof(GetEvent),
					new { id = createdEvent.Id },
					createdEvent);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating event");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] EventUpdateDto eventDto)
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

		[HttpPatch("{id}/status")]
		public async Task<ActionResult<EventBasicDetailedDto>> UpdateEventStatus(Guid id, [FromBody] EventStatusDto statusDto)
		{
			try
			{
				var updatedEvent = await _eventService.UpdateEventStatusAsync(id, statusDto);
				return updatedEvent == null ? NotFound() : Ok(updatedEvent);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating status for event {EventId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpDelete("{id}")]
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

		[HttpGet("{id}/attendee-count")]
		public async Task<ActionResult<int>> GetAttendeeCount(Guid id)
		{
			try
			{
				var count = await _eventService.GetAttendeeCountAsync(id);
				return Ok(count);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting attendee count for event {EventId}", id);
				return StatusCode(500, "Internal server error");
			}
		}
	}
}