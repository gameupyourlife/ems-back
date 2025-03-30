using ems_back.Repo.DTOs;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class TriggersController : ControllerBase
{
	private readonly ITriggerRepository _triggerRepository;
	private readonly ILogger<TriggersController> _logger;

	public TriggersController(
		ITriggerRepository triggerRepository,
		ILogger<TriggersController> logger)
	{
		_triggerRepository = triggerRepository;
		_logger = logger;
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<TriggerDetailedDto>> GetTrigger(Guid id)
	{
		try
		{
			var trigger = await _triggerRepository.GetByIdAsync(id);
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
			var triggers = await _triggerRepository.GetByFlowAsync(flowId);
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
			var triggers = await _triggerRepository.GetByTypeAsync(type);
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
			var createdTrigger = await _triggerRepository.AddAsync(triggerDto);
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

			var updatedTrigger = await _triggerRepository.UpdateAsync(triggerDto);
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
			var result = await _triggerRepository.DeleteAsync(id);
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
