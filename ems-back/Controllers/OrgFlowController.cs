using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Flow.FlowTemplate;
using ems_back.Repo.DTOs.Placeholder;
using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Services;
using ems_back.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Controllers
{
    [Route("api/orgs/{orgId}/flowTemplates")]
    [ApiController]
    public class OrgFlowController : ControllerBase
    {
        private readonly IOrgFlowService _orgFlowService;
        private readonly ILogger<OrgFlowController> _logger;

        public OrgFlowController(
            IOrgFlowService orgFlowService,
            ILogger<OrgFlowController> logger)
        {
            _orgFlowService = orgFlowService;
            _logger = logger;
        }

        // GET: api/orgs/{orgId}/flows
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlowTemplateResponseDto>>> GetAllFlowTemplates([FromRoute] Guid orgId)
        {
            try
            {
                var flowTemplates = await _orgFlowService.GetAllFlowTemplatesAsync(orgId);
                if (flowTemplates == null || !flowTemplates.Any())
                {
                    _logger.LogWarning("No flow templates found for organization with id {OrgId}", orgId);
                    return NotFound("No flow templates found");
                }
                _logger.LogInformation("Flow templates found for organization with id {OrgId}", orgId);
                return Ok(flowTemplates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all flow templates");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/orgs/{orgId}/flows
        [HttpPost]
        public async Task<ActionResult<FlowTemplateResponseDto>> CreateFlow([FromRoute] Guid orgId, [FromBody] FlowTemplateCreateDto flowDto)
        {
            try
            {
                var createdFlowTemplate = await _orgFlowService.CreateFlowTemplateAsync(orgId, flowDto);
                if (createdFlowTemplate == null)
                {
                    _logger.LogWarning("Failed to create flow template");
                    return BadRequest("Failed to create flow template");
                }
                _logger.LogInformation("Flow template created successfully with id {FlowId}", createdFlowTemplate.FlowTemplateId);
                return createdFlowTemplate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating flow template");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/orgs/{orgId}/flows/{flowId}
        [HttpGet("{templateId}")]
        public async Task<ActionResult<FlowTemplateResponseDto>> GetFlowTemplateDetails([FromRoute] Guid orgId, [FromRoute] Guid templateId)
        {
            try
            {
                var flowTemplate = await _orgFlowService.GetFlowTemplateByIdAsync(orgId, templateId);

                if (flowTemplate == null)
                {
                    _logger.LogWarning("Flow template with ID {TemplateId} not found for organization {OrgId}", templateId, orgId);
                    return NotFound($"Flow template with ID {templateId} not found.");
                }

                _logger.LogInformation("Flow template with ID {TemplateId} retrieved for organization {OrgId}", templateId, orgId);
                return Ok(flowTemplate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving flow template with ID {TemplateId}", templateId);
                return StatusCode(500, "Internal server error");
            }
        }


        // PUT: api/orgs/{orgId}/flows/{flowId}
        [HttpPut("{templateId}")]
        public async Task<ActionResult> UpdateFlow(Guid orgId, Guid templateId, [FromBody] FlowTemplateUpdateDto flowTemplateDto)
        {
            if (flowTemplateDto == null)
            {
                _logger.LogWarning("Failed to update flow template");
                return BadRequest("FlowTemplate data is required.");
            }

            try
            {
                var updatedTemplate = await _orgFlowService.UpdateFlowTemplateAsync(orgId, templateId, flowTemplateDto);

                if (updatedTemplate == null)
                {
                    return NotFound($"FlowTemplate with ID {templateId} not found in organization {orgId}.");
                }

                return Ok(updatedTemplate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating flow template with ID {TemplateId}", templateId);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/orgs/{orgId}/flows/{flowId}
        [HttpDelete("{templateId}")]
        public async Task<ActionResult> DeleteFlow([FromRoute] Guid orgId, [FromRoute] Guid templateId)
        {
            try
            {
                var result = await _orgFlowService.DeleteFlowTemplateAsync(orgId, templateId);
                if (!result)
                {
                    return NotFound(new { message = $"Template with ID {templateId} not found." });
                }

                return Ok(new { message = $"Template with ID {templateId} was successfully deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting flow template with ID {TemplateId}", templateId);
                return StatusCode(500, "Internal server error");
            }
        }


        // GET: api/orgs/{orgId}/flows/{flowId}/actions
        [HttpGet("{templateId}/actions")]
        public async Task<ActionResult<IEnumerable<ActionDto>>> GetActions(Guid orgId, Guid templateId)
        {
            try
            {
                var actions = await _orgFlowService.GetActionsForTemplateAsync(orgId, templateId);

                if (actions == null || !actions.Any())
                {
                    _logger.LogWarning("No actions found for template {TemplateId} in organization {OrgId}", templateId, orgId);
                    return NotFound("No actions found for this template");
                }

                _logger.LogInformation("Returned actions for template {TemplateId} in organization {OrgId}", templateId, orgId);
                return Ok(actions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving actions for template {TemplateId}", templateId);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/orgs/{orgId}/flows/{flowId}/actions
        [HttpPost("{templateId}/actions")]
        public async Task<ActionResult<ActionDto>> CreateAction([FromRoute] Guid orgId, [FromRoute] Guid templateId, 
            [FromBody] ActionCreateDto actionCreateDto)
        {
            try
            {
                if (actionCreateDto == null)
                {
                    return BadRequest("Action data is required.");
                }

                if (!Enum.IsDefined(typeof(ActionType), actionCreateDto.Type))
                {
                    return BadRequest("Action type is invalid.");
                }

                var createdAction = await _orgFlowService.CreateActionAsync(orgId, templateId, actionCreateDto);

                return createdAction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating action for template {TemplateId}", templateId);
                return StatusCode(500, "Internal server error");
            }
        }


        // GET: api/orgs/{orgId}/flows/{flowId}/actions/{actionId}
        [HttpGet("{templateId}/actions/{actionId}")]
        public async Task<ActionResult<ActionDto>> GetActionDetails(Guid orgId, Guid templateId, Guid actionId)
        {
            try
            {
                var action = await _orgFlowService.GetActionByIdAsync(orgId, templateId, actionId);

                if (action == null)
                {
                    _logger.LogWarning("Action with ID {ActionId} not found for template {TemplateId} in organization {OrgId}", actionId, templateId, orgId);
                    return NotFound("Action not found for this template");
                }

                _logger.LogInformation("Returned action with ID {ActionId} for template {TemplateId} in organization {OrgId}", actionId, templateId, orgId);
                return Ok(action);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving action with ID {ActionId} for template {TemplateId}", actionId, templateId);
                return StatusCode(500, "Internal server error");
            }
        }


        // PUT: api/orgs/{orgId}/flows/{flowId}/actions/{actionId}
        [HttpPut("{templateId}/actions/{actionId}")]
        public async Task<ActionResult<ActionDto>> UpdateAction(Guid orgId, Guid templateId, Guid actionId, [FromBody] ActionUpdateDto dto)
        {
            if (dto == null)
                return BadRequest("Action data is required.");

            try
            {
                var updatedAction = await _orgFlowService.UpdateActionAsync(orgId, templateId, actionId, dto);
                return Ok(updatedAction); // <- GIBT aktualisierte ActionDto zurück
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating action {ActionId} for template {TemplateId}", actionId, templateId);
                return StatusCode(500, "Internal server error");
            }
        }


        // DELETE: api/orgs/{orgId}/flows/{flowId}/actions/{actionId}
        [HttpDelete("{templateId}/actions/{actionId}")]
        public async Task<ActionResult> DeleteAction(Guid orgId, Guid templateId, Guid actionId)
        {
            try
            {
                var result = await _orgFlowService.DeleteActionAsync(orgId, templateId, actionId);
                if (!result)
                {
                    return NotFound(new { message = $"Action with ID {actionId} not found in template {templateId}." });
                }

                return Ok(new { message = $"Action with ID {actionId} was successfully deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting action with ID {ActionId} for template {TemplateId}", actionId, templateId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/orgs/{orgId}/flows/{flowId}/triggers
        [HttpGet("{templateId}/triggers")]
        public async Task<ActionResult<IEnumerable<TriggerDto>>> GetTriggers(Guid orgId, Guid templateId)
        {
            try
            {
                var triggers = await _orgFlowService.GetTriggersForTemplateAsync(orgId, templateId);

                if (triggers == null || !triggers.Any())
                {
                    _logger.LogWarning("No triggers found for template {TemplateId} in organization {OrgId}", templateId, orgId);
                    return NotFound("No triggers found for this template.");
                }

                _logger.LogInformation("Returned triggers for template {TemplateId} in organization {OrgId}", templateId, orgId);
                return Ok(triggers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving triggers for template {TemplateId}", templateId);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/orgs/{orgId}/flows/{flowId}/triggers
        [HttpPost("{templateId}/triggers")]
        public async Task<ActionResult<TriggerDto>> CreateTrigger(Guid orgId, Guid templateId, [FromBody] TriggerCreateDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Trigger data is required.");

                var createdTrigger = await _orgFlowService.CreateTriggerAsync(orgId, templateId, dto);

                _logger.LogInformation("Created trigger {TriggerId} for template {TemplateId} in organization {OrgId}", createdTrigger.Id, templateId, orgId);

                return CreatedAtAction(nameof(GetTriggerDetails), new { orgId, templateId, triggerId = createdTrigger.Id }, createdTrigger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trigger for template {TemplateId}", templateId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/orgs/{orgId}/flows/{flowId}/triggers/{triggerId}
        [HttpGet("{templateId}/triggers/{triggerId}")]
        public async Task<ActionResult<TriggerDto>> GetTriggerDetails(Guid orgId, Guid templateId, Guid triggerId)
        {
            try
            {
                var trigger = await _orgFlowService.GetTriggerByIdAsync(orgId, templateId, triggerId);

                if (trigger == null)
                {
                    _logger.LogWarning("Trigger {TriggerId} not found for template {TemplateId} in organization {OrgId}", triggerId, templateId, orgId);
                    return NotFound("Trigger not found for this template.");
                }

                _logger.LogInformation("Returned trigger {TriggerId} for template {TemplateId} in organization {OrgId}", triggerId, templateId, orgId);
                return Ok(trigger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving trigger {TriggerId} for template {TemplateId}", triggerId, templateId);
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/orgs/{orgId}/flows/{flowId}/triggers/{triggerId}
        [HttpPut("{templateId}/triggers/{triggerId}")]
        public async Task<ActionResult<TriggerDto>> UpdateTrigger(Guid orgId, Guid templateId, Guid triggerId, [FromBody] TriggerUpdateDto dto)
        {
            if (dto == null)
                return BadRequest("Trigger data is required.");

            try
            {
                var updated = await _orgFlowService.UpdateTriggersAsync(orgId, templateId, triggerId, dto);
                _logger.LogInformation("Updated trigger {TriggerId} for template {TemplateId} in organization {OrgId}", triggerId, templateId, orgId);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Trigger {TriggerId} not found for update in template {TemplateId}", triggerId, templateId);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating trigger {TriggerId} for template {TemplateId}", triggerId, templateId);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/orgs/{orgId}/flows/{flowId}/triggers/{triggerId}
        [HttpDelete("{templateId}/triggers/{triggerId}")]
        public async Task<ActionResult> DeleteTrigger(Guid orgId, Guid templateId, Guid triggerId)
        {
            try
            {
                var result = await _orgFlowService.DeleteTriggerAsync(orgId, templateId, triggerId);
                if (!result)
                {
                    _logger.LogWarning("Trigger {TriggerId} not found for deletion in template {TemplateId}", triggerId, templateId);
                    return NotFound(new { message = $"Trigger with ID {triggerId} not found in template {templateId}." });
                }

                _logger.LogInformation("Deleted trigger {TriggerId} from template {TemplateId}", triggerId, templateId);
                return Ok(new { message = $"Trigger with ID {triggerId} was successfully deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting trigger {TriggerId} from template {TemplateId}", triggerId, templateId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}