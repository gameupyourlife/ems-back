using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AgendaEntriesController : ControllerBase
	{
		private readonly IAgendaEntryRepository _agendaEntryRepository;

		public AgendaEntriesController(IAgendaEntryRepository agendaEntryRepository)
		{
			_agendaEntryRepository = agendaEntryRepository;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<AgendaEntry>> GetAgendaEntry(Guid id)
		{
			var entry = await _agendaEntryRepository.GetByIdAsync(id);
			if (entry == null) return NotFound();
			return Ok(entry);
		}

		[HttpGet("event/{eventId}")]
		public async Task<ActionResult<IEnumerable<AgendaEntry>>> GetAgendaEntriesByEvent(Guid eventId)
		{
			return Ok(await _agendaEntryRepository.GetByEventAsync(eventId));
		}

		[HttpGet("upcoming")]
		public async Task<ActionResult<IEnumerable<AgendaEntry>>> GetUpcomingAgendaEntries([FromQuery] int days = 7)
		{
			return Ok(await _agendaEntryRepository.GetUpcomingEntriesAsync(days));
		}

		[HttpPost]
		public async Task<ActionResult<AgendaEntry>> CreateAgendaEntry([FromBody] AgendaEntry entry)
		{
			var createdEntry = await _agendaEntryRepository.AddAsync(entry);
			return CreatedAtAction(nameof(GetAgendaEntry), new { id = createdEntry.Id }, createdEntry);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAgendaEntry(Guid id, [FromBody] AgendaEntry entry)
		{
			if (id != entry.Id) return BadRequest();
			await _agendaEntryRepository.UpdateAsync(entry);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAgendaEntry(Guid id)
		{
			await _agendaEntryRepository.DeleteAsync(id);
			return NoContent();
		}
	}
}