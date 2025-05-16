using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Flow.FlowTemplate;
using ems_back.Repo.DTOs.Placeholder;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
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
        public async Task<ActionResult<PlaceholderDTO>> GetActions(Guid orgId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        // POST: api/orgs/{orgId}/flows/{flowId}/actions
        [HttpPost("{templateId}/actions")]
        public async Task<ActionResult<PlaceholderDTO>> CreateAction(Guid orgId, Guid flowId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/flows/{flowId}/actions/{actionId}
        [HttpGet("{templateId}/actions/{actionId}")]
        public async Task<ActionResult<PlaceholderDTO>> GetActionDetails(Guid orgId, Guid flowId, Guid actionId)
        {
            throw new NotImplementedException();
        }

        // PUT: api/orgs/{orgId}/flows/{flowId}/actions/{actionId}
        [HttpPut("{templateId}/actions/{actionId}")]
        public async Task<ActionResult> UpdateAction(Guid orgId, Guid flowId, Guid actionId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/orgs/{orgId}/flows/{flowId}/actions/{actionId}
        [HttpDelete("{templateId}/actions/{actionId}")]
        public async Task<ActionResult> DeleteAction(Guid orgId, Guid flowId, Guid actionId)
        {
            throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/flows/{flowId}/triggers
        [HttpGet("{templateId}/triggers")]
        public async Task<ActionResult<PlaceholderDTO>> GetTriggers(Guid orgId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        // POST: api/orgs/{orgId}/flows/{flowId}/triggers
        [HttpPost("{templateId}/triggers")]
        public async Task<ActionResult<PlaceholderDTO>> CreateTrigger(Guid orgId, Guid flowId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/flows/{flowId}/triggers/{triggerId}
        [HttpGet("{templateId}/triggers/{triggerId}")]
        public async Task<ActionResult<PlaceholderDTO>> GetTriggerDetails(Guid orgId, Guid flowId, Guid triggerId)
        {
            throw new NotImplementedException();
        }

        // PUT: api/orgs/{orgId}/flows/{flowId}/triggers/{triggerId}
        [HttpPut("{templateId}/triggers/{triggerId}")]
        public async Task<ActionResult> UpdateTrigger(Guid orgId, Guid flowId, Guid triggerId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/orgs/{orgId}/flows/{flowId}/triggers/{triggerId}
        [HttpDelete("{templateId}/triggers/{triggerId}")]
        public async Task<ActionResult> DeleteTrigger(Guid orgId, Guid flowId, Guid triggerId)
        {
            throw new NotImplementedException();

        }
    }
}