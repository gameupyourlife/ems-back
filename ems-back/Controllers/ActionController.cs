using ems_back.Repo.DTOs;
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
	public class ActionsController : ControllerBase
	{
		private readonly IActionRepository _actionRepository;
		private readonly ILogger<ActionsController> _logger;

		public ActionsController(
			IActionRepository actionRepository,
			ILogger<ActionsController> logger)
		{
			_actionRepository = actionRepository;
			_logger = logger;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ActionDetailedDto>> GetAction(Guid id)
		{
			try
			{
				var action = await _actionRepository.GetByIdAsync(id);
				if (action == null)
				{
					_logger.LogWarning("Action with id {ActionId} not found", id);
					return NotFound();
				}
				return Ok(action);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting action with id {ActionId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("flow/{flowId}")]
		public async Task<ActionResult<IEnumerable<ActionDto>>> GetActionsByFlow(Guid flowId)
		{
			try
			{
				var actions = await _actionRepository.GetByFlowAsync(flowId);
				return Ok(actions);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting actions for flow {FlowId}", flowId);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPost]
		public async Task<ActionResult<ActionDetailedDto>> CreateAction([FromBody] ActionCreateDto actionDto)
		{
			try
			{
				// Validate JSON details
				if (!IsValidJson(actionDto.Details))
				{
					return BadRequest("Details must be valid JSON");
				}

				var createdAction = await _actionRepository.AddAsync(actionDto);
				return CreatedAtAction(
					nameof(GetAction),
					new { id = createdAction.Id },
					createdAction);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating action");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAction(Guid id, [FromBody] ActionUpdateDto actionDto)
		{
			try
			{
				if (id != actionDto.Id)
				{
					return BadRequest("ID mismatch");
				}

				// Validate JSON details if provided
				if (actionDto.Details != null && !IsValidJson(actionDto.Details))
				{
					return BadRequest("Details must be valid JSON");
				}

				var updatedAction = await _actionRepository.UpdateAsync(actionDto);
				if (updatedAction == null)
				{
					return NotFound();
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating action with id {ActionId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAction(Guid id)
		{
			try
			{
				var result = await _actionRepository.DeleteAsync(id);
				if (!result)
				{
					return NotFound();
				}
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting action with id {ActionId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		private bool IsValidJson(string jsonString)
		{
			if (string.IsNullOrWhiteSpace(jsonString))
				return false;

			try
			{
				System.Text.Json.JsonDocument.Parse(jsonString);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}