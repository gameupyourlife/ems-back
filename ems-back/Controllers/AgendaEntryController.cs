using ems_back.Repo.DTOs.Agency;
using ems_back.Repo.DTOs.Agenda;
using ems_back.Repo.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
		private readonly ILogger<AgendaEntriesController> _logger;

		public AgendaEntriesController(
			IAgendaEntryRepository agendaEntryRepository,
			ILogger<AgendaEntriesController> logger)
		{
			_agendaEntryRepository = agendaEntryRepository;
			_logger = logger;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<AgendaEntryDto>> GetAgendaEntry(Guid id)
		{
			try
			{
				var entry = await _agendaEntryRepository.GetByIdAsync(id);
				if (entry == null)
				{
					_logger.LogWarning("Agenda entry with id {EntryId} not found", id);
					return NotFound();
				}
				return Ok(entry);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting agenda entry with id {EntryId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("event/{eventId}")]
		public async Task<ActionResult<IEnumerable<AgendaEntryDto>>> GetAgendaEntriesByEvent(Guid eventId)
		{
			try
			{
				var entries = await _agendaEntryRepository.GetByEventAsync(eventId);
				return Ok(entries);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting agenda entries for event {EventId}", eventId);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("upcoming")]
		public async Task<ActionResult<IEnumerable<AgendaEntryDto>>> GetUpcomingAgendaEntries([FromQuery] int days = 7)
		{
			try
			{
				var entries = await _agendaEntryRepository.GetUpcomingEntriesAsync(days);
				return Ok(entries);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting upcoming agenda entries");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPost]
		public async Task<ActionResult<AgendaEntryDto>> CreateAgendaEntry([FromBody] AgendaEntryCreateDto entryDto)
		{
			try
			{
				// Validate time range
				if (entryDto.Start >= entryDto.End)
				{
					return BadRequest("End time must be after start time");
				}

				var createdEntry = await _agendaEntryRepository.AddAsync(entryDto);
				return CreatedAtAction(
					nameof(GetAgendaEntry),
					new { id = createdEntry.Id },
					createdEntry);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating agenda entry");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAgendaEntry(Guid id, [FromBody] AgendaEntryUpdateDto entryDto)
		{
			try
			{
				if (id != entryDto.Id)
				{
					return BadRequest("ID mismatch");
				}

				// Validate time range if times are being updated
				if (entryDto.Start.HasValue && entryDto.End.HasValue &&
					entryDto.Start.Value >= entryDto.End.Value)
				{
					return BadRequest("End time must be after start time");
				}

				var updatedEntry = await _agendaEntryRepository.UpdateAsync(entryDto);
				if (updatedEntry == null)
				{
					return NotFound();
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating agenda entry with id {EntryId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAgendaEntry(Guid id)
		{
			try
			{
				var result = await _agendaEntryRepository.DeleteAsync(id);
				if (!result)
				{
					return NotFound();
				}
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting agenda entry with id {EntryId}", id);
				return StatusCode(500, "Internal server error");
			}
		}
	}
}