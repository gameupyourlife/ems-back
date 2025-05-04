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
        [HttpGet("{templateId}")]
        public async Task<ActionResult<PlaceholderDTO>> GetFlowDetails(Guid orgId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        // PUT: api/orgs/{orgId}/flows/{flowId}
        [HttpPut("{templateId}")]
        public async Task<ActionResult> UpdateFlow(Guid orgId, Guid flowId, [FromBody] FlowUpdateDto flowDto)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/orgs/{orgId}/flows/{flowId}
        [HttpDelete("{templateId}")]
        public async Task<ActionResult> DeleteFlow(Guid orgId, Guid flowId)
        {
            throw new NotImplementedException();
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