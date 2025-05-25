using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Placeholder;
using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ems_back.Controllers
{
    [Route("api/orgs/{orgId}/events/{eventId}/flows")]
    [ApiController]
    public class EventFlowController : ControllerBase
    {
        private readonly IEventFlowService _eventFlowService;
        private readonly ILogger<EventFlowController> _logger;


        public EventFlowController(
            IEventFlowService eventFlowService,
            ILogger<EventFlowController> logger)
        {
            _eventFlowService = eventFlowService;
            _logger = logger;
        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows
        [HttpGet]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<IEnumerable<FlowOverviewDto>>> GetFlows([FromRoute] Guid orgId, [FromRoute] Guid eventId)
        {
            try
            {
                var flows = await _eventFlowService.GetAllFlows(orgId, eventId);

                if (flows == null || !flows.Any())
                {
                    _logger.LogWarning("No flows found for event {EventId} in organization {OrgId}", eventId, orgId);
                    return Ok(flows);
                }

                _logger.LogInformation("Flows retrieved for event {EventId} in organization {OrgId}", eventId, orgId);
                return Ok(flows);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Event with id {eventId} not found in organization {orgId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving flows for event {EventId}", eventId);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/orgs/{orgId}/events/{eventId}/flows
        [HttpPost]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<FlowOverviewDto>> CreateFlow(Guid orgId, Guid eventId, [FromBody] FlowCreateDto flowDto)
        {
            try
            {
                var createdFlow = await _eventFlowService.CreateFlowAsync(orgId, eventId, flowDto);
                if (createdFlow == null)
                {
                    _logger.LogWarning("Failed to create flow for event {EventId}", eventId);
                    return BadRequest("Flow creation failed");
                }

                _logger.LogInformation("Flow created successfully for event {EventId}", eventId);
                return CreatedAtAction(nameof(GetFlowDetails), new { orgId, eventId, flowId = createdFlow.Id }, createdFlow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating flow for event {EventId}", eventId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows/{flowId}
        [HttpGet("{flowId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<FlowOverviewDto>> GetFlowDetails(Guid orgId, Guid eventId, Guid flowId)
        {
            try
            {
                var flow = await _eventFlowService.GetFlowByIdAsync(orgId, eventId, flowId);
                if (flow == null)
                {
                    _logger.LogWarning("Flow {FlowId} not found for event {EventId}", flowId, eventId);
                    return Ok(flow);
                }

                _logger.LogInformation("Flow {FlowId} retrieved for event {EventId}", flowId, eventId);
                return Ok(flow);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Event with id {eventId} not found in organization {orgId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving flow {FlowId} for event {EventId}", flowId, eventId);
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/orgs/{orgId}/events/{eventId}/flows/{flowId}
        [HttpPut("{flowId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<FlowOverviewDto>> UpdateFlow(Guid orgId, Guid eventId, Guid flowId, [FromBody] FlowUpdateDto updateDto)
        {
            try
            {
                var updated = await _eventFlowService.UpdateFlow(orgId, eventId, flowId, updateDto);
                if (updated == null)
                {
                    _logger.LogWarning("Flow {FlowId} not found for update in event {EventId}", flowId, eventId);
                    return NotFound($"Flow {flowId} not found.");
                }

                _logger.LogInformation("Flow {FlowId} updated for event {EventId}", flowId, eventId);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating flow {FlowId} for event {EventId}", flowId, eventId);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}/flows/{flowId}
        [HttpDelete("{flowId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult> DeleteFlow(Guid orgId, Guid eventId, Guid flowId)
        {
            try
            {
                var result = await _eventFlowService.DeleteFlow(orgId, eventId, flowId);
                if (!result)
                {
                    _logger.LogWarning("Flow {FlowId} not found for deletion in event {EventId}", flowId, eventId);
                    return NotFound(new { message = $"Flow {flowId} not found." });
                }

                _logger.LogInformation("Flow {FlowId} deleted from event {EventId}", flowId, eventId);
                return Ok(new { message = $"Flow {flowId} was successfully deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting flow {FlowId} from event {EventId}", flowId, eventId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/actions
        [HttpGet("{flowId}/actions")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<IEnumerable<ActionDto>>> GetActions(Guid orgId, Guid eventId, Guid flowId)
        {
            try
            {
                var actions = await _eventFlowService.GetActionsForFlowAsync(eventId, flowId);
                if (actions == null || !actions.Any())
                {
                    _logger.LogWarning("No actions found for flow {FlowId} in event {EventId}", flowId, eventId);
                    return Ok(actions);
                }

                _logger.LogInformation("Actions retrieved for flow {FlowId} in event {EventId}", flowId, eventId);
                return Ok(actions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving actions for flow {FlowId}", flowId);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/actions
        [HttpPost("{flowId}/actions")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<ActionDto>> CreateAction(Guid orgId, Guid eventId, Guid flowId, [FromBody] ActionCreateDto actionDto)
        {
            try
            {
                if (!Enum.IsDefined(typeof(ActionType), actionDto.Type))
                {
                    return BadRequest("Invalid action type.");
                }

                var createdAction = await _eventFlowService.CreateActionAsync(eventId, flowId, actionDto);
                _logger.LogInformation("Action created for flow {FlowId} in event {EventId}", flowId, eventId);

                return CreatedAtAction(nameof(GetActionDetails), new { orgId, eventId, flowId, actionId = createdAction.Id }, createdAction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating action for flow {FlowId}", flowId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/actions/{actionId}
        [HttpGet("{flowId}/actions/{actionId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<ActionDto>> GetActionDetails(Guid orgId, Guid eventId, Guid flowId, Guid actionId)
        {
            try
            {
                var action = await _eventFlowService.GetActionByIdAsync(eventId, flowId, actionId);
                if (action == null)
                {
                    return Ok(action);
                }

                return Ok(action);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving action {ActionId} for flow {FlowId}", actionId, flowId);
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/actions/{actionId}
        [HttpPut("{flowId}/actions/{actionId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<ActionDto>> UpdateAction(Guid orgId, Guid eventId, Guid flowId, Guid actionId, [FromBody] ActionUpdateDto dto)
        {
            try
            {
                var updated = await _eventFlowService.UpdateActionAsync(eventId, flowId, actionId, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating action {ActionId} for flow {FlowId}", actionId, flowId);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/actions/{actionId}
        [HttpDelete("{flowId}/actions/{actionId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult> DeleteAction(Guid orgId, Guid eventId, Guid flowId, Guid actionId)
        {
            try
            {
                var result = await _eventFlowService.DeleteActionAsync(eventId, flowId, actionId);
                if (!result)
                {
                    return NotFound(new { message = $"Action {actionId} not found in flow {flowId}." });
                }

                return Ok(new { message = $"Action {actionId} successfully deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting action {ActionId} for flow {FlowId}", actionId, flowId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/triggers
        [HttpGet("{flowId}/triggers")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<IEnumerable<TriggerDto>>> GetTriggers(Guid orgId, Guid eventId, Guid flowId)
        {
            try
            {
                var triggers = await _eventFlowService.GetTriggersForFlowAsync(orgId, eventId, flowId);
                if (triggers == null || !triggers.Any())
                {
                    return Ok(triggers);
                }

                return Ok(triggers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving triggers for flow {FlowId}", flowId);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/triggers
        [HttpPost("{flowId}/triggers")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<TriggerDto>> CreateTrigger(Guid orgId, Guid eventId, Guid flowId, [FromBody] TriggerCreateDto dto)
        {
            try
            {
                var created = await _eventFlowService.CreateTriggerAsync(eventId, flowId, dto);
                return CreatedAtAction(nameof(GetTriggerDetails), new { orgId, eventId, flowId, triggerId = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trigger for flow {FlowId}", flowId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/triggers/{triggerId}
        [HttpGet("{flowId}/triggers/{triggerId}")]
        public async Task<ActionResult<TriggerDto>> GetTriggerDetails(Guid orgId, Guid eventId, Guid flowId, Guid triggerId)
        {
            try
            {
                var trigger = await _eventFlowService.GetTriggerByIdAsync(eventId, flowId, triggerId);
                if (trigger == null)
                {
                    return Ok(trigger);
                }

                return Ok(trigger);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Trigger not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving trigger {TriggerId} for flow {FlowId}", triggerId, flowId);
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/triggers/{triggerId}
        [HttpPut("{flowId}/triggers/{triggerId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<TriggerDto>> UpdateTrigger(Guid orgId, Guid eventId, Guid flowId, Guid triggerId, [FromBody] TriggerUpdateDto dto)
        {
            try
            {
                var updated = await _eventFlowService.UpdateTriggerAsync(eventId, flowId, triggerId, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating trigger {TriggerId} for flow {FlowId}", triggerId, flowId);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/triggers/{triggerId}
        [HttpDelete("{flowId}/triggers/{triggerId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult> DeleteTrigger(Guid orgId, Guid eventId, Guid flowId, Guid triggerId)
        {
            try
            {
                var result = await _eventFlowService.DeleteTriggerAsync(eventId, flowId, triggerId);
                if (!result)
                {
                    return NotFound(new { message = $"Trigger {triggerId} not found." });
                }

                return Ok(new { message = $"Trigger {triggerId} successfully deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting trigger {TriggerId} from flow {FlowId}", triggerId, flowId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}