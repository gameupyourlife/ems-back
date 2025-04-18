using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Placeholder;
using ems_back.Repo.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Controllers
{
    [Route("api/orgs/{orgId}/flows")]
    [ApiController]
    public class FlowsController : ControllerBase
    {

        public FlowsController()
        {

        }

        // GET: api/orgs/{orgId}/flows
        [HttpGet]
        public async Task<ActionResult<PlaceholderDTO>> GetAllFlows(Guid orgId)
        {
            throw new NotImplementedException();
        }

        // POST: api/orgs/{orgId}/flows
        [HttpPost]
        public async Task<ActionResult<PlaceholderDTO>> CreateFlow(Guid orgId, [FromBody] FlowCreateDto flowDto)
        {
            throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/flows/{flowId}
        [HttpGet("{flowId}")]
        public async Task<ActionResult<PlaceholderDTO>> GetFlowDetails(Guid orgId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        // PUT: api/orgs/{orgId}/flows/{flowId}
        [HttpPut("{flowId}")]
        public async Task<ActionResult> UpdateFlow(Guid orgId, Guid flowId, [FromBody] FlowUpdateDto flowDto)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/orgs/{orgId}/flows/{flowId}
        [HttpDelete("{flowId}")]
        public async Task<ActionResult> DeleteFlow(Guid orgId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/flows/{flowId}/actions
        [HttpGet("{flowId}/actions")]
        public async Task<ActionResult<PlaceholderDTO>> GetActions(Guid orgId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        // POST: api/orgs/{orgId}/flows/{flowId}/actions
        [HttpPost("{flowId}/actions")]
        public async Task<ActionResult<PlaceholderDTO>> CreateAction(Guid orgId, Guid flowId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/flows/{flowId}/actions/{actionId}
        [HttpGet("{flowId}/actions/{actionId}")]
        public async Task<ActionResult<PlaceholderDTO>> GetActionDetails(Guid orgId, Guid flowId, Guid actionId)
        {
            throw new NotImplementedException();
        }

        // PUT: api/orgs/{orgId}/flows/{flowId}/actions/{actionId}
        [HttpPut("{flowId}/actions/{actionId}")]
        public async Task<ActionResult> UpdateAction(Guid orgId, Guid flowId, Guid actionId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/orgs/{orgId}/flows/{flowId}/actions/{actionId}
        [HttpDelete("{flowId}/actions/{actionId}")]
        public async Task<ActionResult> DeleteAction(Guid orgId, Guid flowId, Guid actionId)
        {
            throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/flows/{flowId}/triggers
        [HttpGet("{flowId}/triggers")]
        public async Task<ActionResult<PlaceholderDTO>> GetTriggers(Guid orgId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        // POST: api/orgs/{orgId}/flows/{flowId}/triggers
        [HttpPost("{flowId}/triggers")]
        public async Task<ActionResult<PlaceholderDTO>> CreateTrigger(Guid orgId, Guid flowId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // GET: api/orgs/{orgId}/flows/{flowId}/triggers/{triggerId}
        [HttpGet("{flowId}/triggers/{triggerId}")]
        public async Task<ActionResult<PlaceholderDTO>> GetTriggerDetails(Guid orgId, Guid flowId, Guid triggerId)
        {
            throw new NotImplementedException();
        }

        // PUT: api/orgs/{orgId}/flows/{flowId}/triggers/{triggerId}
        [HttpPut("{flowId}/triggers/{triggerId}")]
        public async Task<ActionResult> UpdateTrigger(Guid orgId, Guid flowId, Guid triggerId, [FromBody] PlaceholderDTO dtoName)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/orgs/{orgId}/flows/{flowId}/triggers/{triggerId}
        [HttpDelete("{flowId}/triggers/{triggerId}")]
        public async Task<ActionResult> DeleteTrigger(Guid orgId, Guid flowId, Guid triggerId)
        {
            throw new NotImplementedException();

        }
    }
}