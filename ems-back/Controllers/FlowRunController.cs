using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Flow.FlowsRun;
using ems_back.Repo.DTOs.Placeholder;
using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ems_back.Controllers
{
    [Route("api/orgs/{orgId}/FlowRuns")]
    [ApiController]
    public class FlowRunController : ControllerBase
    {
        private readonly IFlowRunService _flowRunService;
        private readonly ILogger<FlowRunController> _logger;


        public FlowRunController(
            IFlowRunService flowRunService,
            ILogger<FlowRunController> logger)
        {
            _flowRunService = flowRunService;
            _logger = logger;
        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows
        [HttpGet]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<IEnumerable<FlowsRunResponseDto>>> GetFlowRuns([FromRoute] Guid orgId)
        {
            try
            {
                var flowRuns = await _flowRunService.GetAllForOrganizationAsync(orgId);

                if (flowRuns == null || !flowRuns.Any())
                {
                    _logger.LogWarning("No flows runs found for organization {OrgId}", orgId);
                    return Ok(flowRuns);
                }

                _logger.LogInformation("FlowsRuns retrieved for organization {OrgId}", orgId);
                return Ok(flowRuns);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Org with with id {orgId} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving flow runs for org {OrgId}", orgId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows/{flowId}
        [HttpGet("{eventId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)}, {nameof(UserRole.Owner)}, {nameof(UserRole.Organizer)}, {nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<FlowOverviewDto>> GetFlowRunsForEvent(Guid orgId, Guid eventId, Guid flowId)
        {
            try
            {
                var flow = await _flowRunService.GetByEventAsync(orgId, eventId);
                if (flow == null)
                {
                    _logger.LogWarning("No FlowRuns not found for event {EventId}", eventId);
                    return Ok(flow);
                }

                _logger.LogInformation("FlowRuns retrieved for event {EventId}", eventId);
                return Ok(flow);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Event with id {eventId} not found in organization {orgId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Flow Runs for event {EventId}", eventId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}