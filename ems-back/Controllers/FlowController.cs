using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Services.Interfaces;
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
		private readonly IFlowService _flowService;
		private readonly ILogger<FlowsController> _logger;

		public FlowsController(
			IFlowService flowService,
			ILogger<FlowsController> logger)
		{
			_flowService = flowService;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<FlowBasicDto>>> GetAllFlows()
		{
			try
			{
				var flows = await _flowService.GetAllActiveFlowsAsync();
				return Ok(flows);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting all flows");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<FlowResponseDto>> GetFlowById(Guid id)
		{
			try
			{
				var flow = await _flowService.GetFlowByIdAsync(id);
				return flow == null ? NotFound() : Ok(flow);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting flow with id {FlowId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("{id}/details")]
		public async Task<ActionResult<FlowDetailedDto>> GetFlowDetails(Guid id)
		{
			try
			{
				var flow = await _flowService.GetFlowDetailsAsync(id);
				return flow == null ? NotFound() : Ok(flow);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting flow details with id {FlowId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPost]
		public async Task<ActionResult<FlowResponseDto>> CreateFlow([FromBody] FlowCreateDto flowDto)
		{
			try
			{
				var createdFlow = await _flowService.CreateFlowAsync(flowDto);
				return CreatedAtAction(
					nameof(GetFlowById),
					new { id = createdFlow.Id },
					createdFlow);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating flow");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateFlow(Guid id, [FromBody] FlowUpdateDto flowDto)
		{
			try
			{
				if (id != flowDto.Id)
				{
					return BadRequest("ID mismatch");
				}

				var success = await _flowService.UpdateFlowAsync(id, flowDto);
				return success ? NoContent() : NotFound();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating flow with id {FlowId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPatch("{id}/toggle-status")]
		public async Task<ActionResult<FlowResponseDto>> ToggleFlowStatus(Guid id, [FromBody] FlowStatusDto statusDto)
		{
			try
			{
				var updatedFlow = await _flowService.ToggleFlowStatusAsync(id, statusDto);
				return updatedFlow == null ? NotFound() : Ok(updatedFlow);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error toggling status for flow with id {FlowId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteFlow(Guid id)
		{
			try
			{
				var success = await _flowService.DeleteFlowAsync(id);
				return success ? NoContent() : NotFound();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting flow with id {FlowId}", id);
				return StatusCode(500, "Internal server error");
			}
		}
	}
}