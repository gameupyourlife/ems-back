using ems_back.Repo.DTOs;
using ems_back.Repo.Interfaces;
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
		private readonly ILogger<FlowsController> _logger;

		public FlowsController(
			IFlowRepository flowRepository,
			ILogger<FlowsController> logger)
		{
			_flowRepository = flowRepository;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<FlowBasicDto>>> GetAllFlows()
		{
			try
			{
				var flows = await _flowRepository.GetAllActiveAsync();
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
				var flow = await _flowRepository.GetByIdAsync(id);
				if (flow == null)
				{
					_logger.LogWarning("Flow with id {FlowId} not found", id);
					return NotFound();
				}
				return Ok(flow);
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
				var flow = await _flowRepository.GetWithDetailsAsync(id);
				if (flow == null)
				{
					_logger.LogWarning("Detailed flow with id {FlowId} not found", id);
					return NotFound();
				}
				return Ok(flow);
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
				var createdFlow = await _flowRepository.AddAsync(flowDto);
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

				var updatedFlow = await _flowRepository.UpdateAsync(id, flowDto);
				if (updatedFlow == null)
				{
					return NotFound();
				}

				return NoContent();
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
				var updatedFlow = await _flowRepository.ToggleStatusAsync(id, statusDto);
				if (updatedFlow == null)
				{
					return NotFound();
				}
				return Ok(updatedFlow);
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
				var result = await _flowRepository.DeleteAsync(id);
				if (!result)
				{
					return NotFound();
				}
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting flow with id {FlowId}", id);
				return StatusCode(500, "Internal server error");
			}
		}
	}
}