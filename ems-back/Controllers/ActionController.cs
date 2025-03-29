using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Action = ems_back.Repo.Models.Action;

namespace ems_back.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ActionsController : ControllerBase
	{
		private readonly IActionRepository _actionRepository;

		public ActionsController(IActionRepository actionRepository)
		{
			_actionRepository = actionRepository;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Action>> GetAction(Guid id)
		{
			var action = await _actionRepository.GetByIdAsync(id);
			if (action == null) return NotFound();
			return Ok(action);
		}

		[HttpGet("flow/{flowId}")]
		public async Task<ActionResult<IEnumerable<Action>>> GetActionsByFlow(Guid flowId)
		{
			return Ok(await _actionRepository.GetByFlowAsync(flowId));
		}

		[HttpPost]
		public async Task<ActionResult<Action>> CreateAction([FromBody] Action action)
		{
			action.CreatedAt = DateTime.UtcNow;
			var createdAction = await _actionRepository.AddAsync(action);
			return CreatedAtAction(nameof(GetAction), new { id = createdAction.Id }, createdAction);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAction(Guid id, [FromBody] Action action)
		{
			if (id != action.Id) return BadRequest();
			await _actionRepository.UpdateAsync(action);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAction(Guid id)
		{
			await _actionRepository.DeleteAsync(id);
			return NoContent();
		}
	}
}