using ems_back.Repo.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ems_back.Repo.Models;
using ems_back.Repo.DTOs.Event;

namespace ems_back.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class EventsController : ControllerBase
	{
		private readonly IEventRepository _eventRepository;
		private readonly ILogger<EventsController> _logger;

		public EventsController(
			IEventRepository eventRepository,
			ILogger<EventsController> logger)
		{
			_eventRepository = eventRepository;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<EventBasicDto>>> GetAllEvents()
		{
			try
			{
				var events = await _eventRepository.GetAllEventsAsync();
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
				var events = await _eventRepository.GetUpcomingEventsAsync(days);
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
				var eventEntity = await _eventRepository.GetByIdAsync(id);
				if (eventEntity == null)
				{
					_logger.LogWarning("Event with id {EventId} not found", id);
					return NotFound();
				}
				return Ok(eventEntity);
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
				var eventEntity = await _eventRepository.GetEventWithAttendeesAsync(id);
				if (eventEntity == null)
				{
					return NotFound();
				}
				return Ok(eventEntity);
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
				var eventEntity = await _eventRepository.GetEventWithAgendaAsync(id);
				if (eventEntity == null)
				{
					return NotFound();
				}
				return Ok(eventEntity);
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
				var eventEntity = await _eventRepository.GetEventWithAllDetailsAsync(id);
				if (eventEntity == null)
				{
					return NotFound();
				}
				return Ok(eventEntity);
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
				var events = await _eventRepository.GetEventsByOrganizationAsync(organizationId);
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
				var events = await _eventRepository.GetEventsByCreatorAsync(userId);
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
				var events = await _eventRepository.GetEventsByCategoryAsync(category);
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
				var events = await _eventRepository.GetEventsByDateRangeAsync(start, end);
				return Ok(events);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting events between {StartDate} and {EndDate}", start, end);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPost]
		public async Task<ActionResult<EventBasicDetailedDto>> CreateEvent([FromBody] EventCreateDto eventDto)
		{
			try
			{
				var createdEvent = await _eventRepository.AddAsync(eventDto);
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

				var updatedEvent = await _eventRepository.UpdateAsync(eventDto);
				if (updatedEvent == null)
				{
					return NotFound();
				}

				return NoContent();
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
				var updatedEvent = await _eventRepository.UpdateStatusAsync(id, statusDto);
				if (updatedEvent == null)
				{
					return NotFound();
				}
				return Ok(updatedEvent);
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
				var result = await _eventRepository.DeleteAsync(id);
				if (!result)
				{
					return NotFound();
				}
				return NoContent();
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
				var count = await _eventRepository.GetAttendeeCountAsync(id);
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