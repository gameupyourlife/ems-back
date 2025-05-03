using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;
using ems_back.Repo.DTOs.Placeholder;
using ems_back.Repo.Interfaces.Repository;

namespace ems_back.Controllers
{
    [Route("api/orgs/{orgId}/[controller]")]
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

        // GET: api/orgs/{orgId}/events
        [HttpGet]
		public async Task<ActionResult<IEnumerable<EventBasicDto>>> GetEvents(Guid orgId)
        {
            try
            {
                var events = await _eventRepository.GetEventsByOrganizationAsync(orgId);
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting events for organization {OrganizationId}", orgId);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/orgs/{orgId}/events
        [HttpPost]
        public async Task<ActionResult<EventBasicDetailedDto>> CreateEvent(Guid orgId, [FromBody] EventCreateDto eventDto)
        {
			throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/events/{eventId}
        [HttpGet("{eventId}")]
		public async Task<ActionResult<EventBasicDetailedDto>> GetEvent(Guid orgId, Guid eventId)
		{
			throw new NotImplementedException();
		}

        // PUT: api/orgs/{orgId}/events/{eventId}
        [HttpPut("{eventId}")]
        public async Task<IActionResult> UpdateEvent(Guid orgId, Guid eventId, [FromBody] EventUpdateDto eventDto)
        {
			throw new NotImplementedException();
        }

		// DELETE: api/orgs/{orgId}/events/{eventId}
		[HttpDelete("{eventId}")]
		public async Task<IActionResult> DeleteEvent(Guid orgId, Guid eventId)
        {
			throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/events/{eventId}/attendees
        [HttpGet("{eventId}/attendees")]
		public async Task<ActionResult<EventBasicDetailedDto>> GetEventWithAttendees(Guid orgId, Guid eventId)
		{
			throw new NotImplementedException();
		}

		// POST: api/orgs/{orgId}/events/{eventId}/attendees
		[HttpPost("{eventId}/attendees")]
		public async Task<ActionResult<PlaceholderDTO>> CreateAttendee(Guid orgId, Guid eventId, [FromBody] PlaceholderDTO dtoName)
		{
			throw new NotImplementedException();
		}

		// DELETE: api/orgs/{orgId}/events/{eventId}/attendees/{userId}
		[HttpDelete("{eventId}/attendees/{userId}")]
		public async Task<ActionResult<PlaceholderDTO>> DeleteAttendee(Guid orgId, Guid eventId)
		{
			throw new NotImplementedException();
		}


        // GET: api/orgs/{orgId}/events/{eventId}/agenda
        [HttpGet("{eventId}/agenda")]
		public async Task<ActionResult<PlaceholderDTO>> GetAgenda(Guid orgId, Guid eventId)
		{
			throw new NotImplementedException();
		}

		// POST: api/orgs/{orgId}/events/{eventId}/agenda
		[HttpPost("{eventId}/agenda")]
        public async Task<ActionResult<PlaceholderDTO>> CreateAgenda(Guid orgId, Guid eventId, [FromBody] PlaceholderDTO dtoName)
		{
			throw new NotImplementedException();
		}

		// GET: api/orgs/{orgId}/events/{eventId}/agenda/{agendaId}
		[HttpGet("{eventId}/agenda/{agendaId}")]
        public async Task<ActionResult<PlaceholderDTO>> GetAgendaById(Guid orgId, Guid eventId, Guid agendaId)
		{
            throw new NotImplementedException();
        }

        // PUT: api/orgs/{orgId}/events/{eventId}/agenda/{agendaId}
        [HttpPut("{eventId}/agenda/{agendaId}")]
        public async Task<IActionResult> UpdateAgenda(Guid orgId, Guid eventId, Guid agendaId, [FromBody] PlaceholderDTO agendaDto)
		{
            throw new NotImplementedException();
        }
        // DELETE: api/orgs/{orgId}/events/{eventId}/agenda/{agendaId}
        [HttpDelete("{eventId}/agenda/{agendaId}")]
        public async Task<IActionResult> DeleteAgenda(Guid orgId, Guid eventId, Guid agendaId)
        {
            throw new NotImplementedException();
        }

		// GET: api/orgs/{orgId}/events/{eventId}/files
		[HttpGet("{eventId}/files")]
		public async Task<ActionResult<PlaceholderDTO>> GetFiles(Guid orgId, Guid eventId)
		{
            throw new NotImplementedException();
        }

        // POST: api/orgs/{orgId}/events/{eventId}/files
        [HttpPost("{eventId}/files")]
        public async Task<ActionResult<PlaceholderDTO>> UploadFile(Guid orgId, Guid eventId, [FromBody] PlaceholderDTO fileDto)
		{
            throw new NotImplementedException();
        }

        // PUT: api/orgs/{orgId}/events/{eventId}/files/{fileId}
        [HttpPut("{eventId}/files/{fileId}")]
        public async Task<ActionResult> DeleteFile(Guid orgId, Guid eventId, Guid fileId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}/files/{fileId}
        [HttpDelete("{eventId}/files/{fileId}")]
        public async Task<IActionResult> DeleteFile(Guid orgId, Guid eventId, Guid fileId)
        {
            throw new NotImplementedException();
        }
    }
}