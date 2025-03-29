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
	public class FlowsController : ControllerBase
	{
		private readonly IFlowRepository _flowRepository;

		public FlowsController(IFlowRepository flowRepository)
		{
			_flowRepository = flowRepository;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Flow>>> GetFlows()
		{
			return Ok(await _flowRepository.GetAllActiveAsync());
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Flow>> GetFlow(Guid id)
		{
			var flow = await _flowRepository.GetByIdAsync(id);
			if (flow == null) return NotFound();
			return Ok(flow);
		}

		[HttpGet("{id}/details")]
		public async Task<ActionResult<Flow>> GetFlowWithDetails(Guid id)
		{
			var flow = await _flowRepository.GetWithDetailsAsync(id);
			if (flow == null) return NotFound();
			return Ok(flow);
		}

		[HttpPost]
		public async Task<ActionResult<Flow>> CreateFlow([FromBody] Flow flow)
		{
			flow.CreatedAt = DateTime.UtcNow;
			flow.UpdatedAt = DateTime.UtcNow;
			var createdFlow = await _flowRepository.AddAsync(flow);
			return CreatedAtAction(nameof(GetFlow), new { id = createdFlow.Id }, createdFlow);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateFlow(Guid id, [FromBody] Flow flow)
		{
			if (id != flow.Id) return BadRequest();
			await _flowRepository.UpdateAsync(flow);
			return NoContent();
		}

		[HttpPatch("{id}/toggle-status")]
		public async Task<IActionResult> ToggleFlowStatus(Guid id)
		{
			await _flowRepository.ToggleStatusAsync(id);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteFlow(Guid id)
		{
			await _flowRepository.DeleteAsync(id);
			return NoContent();
		}
	}
}