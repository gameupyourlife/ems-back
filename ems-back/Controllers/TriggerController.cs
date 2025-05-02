using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class TriggersController : ControllerBase
{
	private readonly ITriggerService _triggerService;
	private readonly ILogger<TriggersController> _logger;

	public TriggersController(
		ITriggerService triggerService,
		ILogger<TriggersController> logger)
	{
		_triggerService = triggerService;
		_logger = logger;
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<TriggerDetailedDto>> GetTrigger(Guid id)
	{
		try
		{
			var trigger = await _triggerService.GetByIdAsync(id);
			if (trigger == null) return NotFound();
			return Ok(trigger);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting trigger with id {TriggerId}", id);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpGet("flow/{flowId}")]
	public async Task<ActionResult<IEnumerable<TriggerDto>>> GetTriggersByFlow(Guid flowId)
	{
		try
		{
			var triggers = await _triggerService.GetByFlowAsync(flowId);
			return Ok(triggers);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting triggers for flow {FlowId}", flowId);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpGet("type/{type}")]
	public async Task<ActionResult<IEnumerable<TriggerDto>>> GetTriggersByType(TriggerType type)
	{
		try
		{
			var triggers = await _triggerService.GetByTypeAsync(type);
			return Ok(triggers);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting triggers of type {TriggerType}", type);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpPost]
	public async Task<ActionResult<TriggerDetailedDto>> CreateTrigger([FromBody] TriggerCreateDto triggerDto)
	{
		try
		{
			var createdTrigger = await _triggerService.AddAsync(triggerDto);
			return CreatedAtAction(
				nameof(GetTrigger),
				new { id = createdTrigger.Id },
				createdTrigger);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error creating trigger");
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateTrigger(Guid id, [FromBody] TriggerUpdateDto triggerDto)
	{
		try
		{
			if (id != triggerDto.Id) return BadRequest("ID mismatch");

			var updatedTrigger = await _triggerService.UpdateAsync(triggerDto);
			if (updatedTrigger == null) return NotFound();

			return NoContent();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating trigger with id {TriggerId}", id);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteTrigger(Guid id)
	{
		try
		{
			var result = await _triggerService.DeleteAsync(id);
			if (!result) return NotFound();
			return NoContent();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting trigger with id {TriggerId}", id);
			return StatusCode(500, "Internal server error");
		}
	}
}
