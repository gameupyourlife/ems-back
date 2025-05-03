using ems_back.Repo.DTOs.Placeholder;
using Microsoft.AspNetCore.Mvc;

namespace ems_back.Controllers
{
    [Route("api/orgs/{orgId}/events/{eventId}/flows")]
    [ApiController]
    public class EventFlowController : ControllerBase
    {
        public EventFlowController()
        {

        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows
        [HttpGet]
        public async Task<ActionResult<PlaceholderDTO>> GetFlows(Guid orgId, Guid eventId)
        {
            throw new NotImplementedException();
        }

        // POST: api/orgs/{orgId}/events/{eventId}/flows
        [HttpPost]
        public async Task<ActionResult<PlaceholderDTO>> CreateFlow(Guid orgId, Guid eventId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows/{flowId}
        [HttpGet("{flowId}")]
        public async Task<ActionResult<PlaceholderDTO>> GetFlowDetails(Guid orgId, Guid eventId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        // PUT: api/orgs/{orgId}/events/{eventId}/flows/{flowId}
        [HttpPut("{flowId}")]
        public async Task<ActionResult> UpdateFlow(Guid orgId, Guid eventId, Guid flowId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}/flows/{flowId}
        [HttpDelete("{flowId}")]
        public async Task<ActionResult> DeleteFlow(Guid orgId, Guid eventId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/actions
        [HttpGet("{flowId}/actions")]
        public async Task<ActionResult<PlaceholderDTO>> GetActions(Guid orgId, Guid eventId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        // POST: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/actions
        [HttpPost("{flowId}/actions")]
        public async Task<ActionResult<PlaceholderDTO>> CreateAction(Guid orgId, Guid eventId, Guid flowId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/actions/{actionId}
        [HttpGet("{flowId}/actions/{actionId}")]
        public async Task<ActionResult<PlaceholderDTO>> GetActionDetails(Guid orgId, Guid eventId, Guid flowId, Guid actionId)
        {
            throw new NotImplementedException();
        }

        // PUT: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/actions/{actionId}
        [HttpPut("{flowId}/actions/{actionId}")]
        public async Task<ActionResult> UpdateAction(Guid orgId, Guid eventId, Guid flowId, Guid actionId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/actions/{actionId}
        [HttpDelete("{flowId}/actions/{actionId}")]
        public async Task<ActionResult> DeleteAction(Guid orgId, Guid eventId, Guid flowId, Guid actionId)
        {
            throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/triggers
        [HttpGet("{flowId}/triggers")]
        public async Task<ActionResult<PlaceholderDTO>> GetTriggers(Guid orgId, Guid eventId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        // POST: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/triggers
        [HttpPost("{flowId}/triggers")]
        public async Task<ActionResult<PlaceholderDTO>> CreateTrigger(Guid orgId, Guid eventId, Guid flowId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/triggers/{triggerId}
        [HttpGet("{flowId}/triggers/{triggerId}")]
        public async Task<ActionResult<PlaceholderDTO>> GetTriggerDetails(Guid orgId, Guid eventId, Guid flowId, Guid triggerId)
        {
            throw new NotImplementedException();
        }

        // PUT: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/triggers/{triggerId}
        [HttpPut("{flowId}/triggers/{triggerId}")]
        public async Task<ActionResult> UpdateTrigger(Guid orgId, Guid eventId, Guid flowId, Guid triggerId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/orgs/{orgId}/events/{eventId}/flows/{flowId}/triggers/{triggerId}
        [HttpDelete("{flowId}/triggers/{triggerId}")]
        public async Task<ActionResult> DeleteTrigger(Guid orgId, Guid eventId, Guid flowId, Guid triggerId)
        {
            throw new NotImplementedException();
        }
    }
}