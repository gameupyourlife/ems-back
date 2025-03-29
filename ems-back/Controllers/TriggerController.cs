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
	public class TriggersController : ControllerBase
	{
		private readonly ITriggerRepository _triggerRepository;

		public TriggersController(ITriggerRepository triggerRepository)
		{
			_triggerRepository = triggerRepository;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Trigger>> GetTrigger(Guid id)
		{
			var trigger = await _triggerRepository.GetByIdAsync(id);
			if (trigger == null) return NotFound();
			return Ok(trigger);
		}

		[HttpGet("flow/{flowId}")]
		public async Task<ActionResult<IEnumerable<Trigger>>> GetTriggersByFlow(Guid flowId)
		{
			return Ok(await _triggerRepository.GetByFlowAsync(flowId));
		}

		[HttpGet("type/{type}")]
		public async Task<ActionResult<IEnumerable<Trigger>>> GetTriggersByType(TriggerType type)
		{
			return Ok(await _triggerRepository.GetByTypeAsync(type));
		}

		[HttpPost]
		public async Task<ActionResult<Trigger>> CreateTrigger([FromBody] Trigger trigger)
		{
			trigger.CreatedAt = DateTime.UtcNow;
			var createdTrigger = await _triggerRepository.AddAsync(trigger);
			return CreatedAtAction(nameof(GetTrigger), new { id = createdTrigger.Id }, createdTrigger);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTrigger(Guid id, [FromBody] Trigger trigger)
		{
			if (id != trigger.Id) return BadRequest();
			await _triggerRepository.UpdateAsync(trigger);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTrigger(Guid id)
		{
			await _triggerRepository.DeleteAsync(id);
			return NoContent();
		}
	}
}