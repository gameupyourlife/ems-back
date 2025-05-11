using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using Microsoft.Extensions.Logging;

namespace ems_back.Repo.Services
{
    public class EventFlowService : IEventFlowService
    {
        private readonly IEventFlowRepository _eventFlowRepository;
        private readonly ILogger<EventFlowService> _logger;

        public EventFlowService(
            IEventFlowRepository eventFlowRepository,
            ILogger<EventFlowService> logger)
        {
            _eventFlowRepository = eventFlowRepository;
            _logger = logger;
        }

        public Task<IEnumerable<FlowOverviewDto>> GetAllFlows(Guid orgId, Guid eventId)
        {
            var flowList = _eventFlowRepository.GetAllFlows(orgId, eventId);
            if (flowList == null)
            {
                _logger.LogWarning("No flows found for organization with id {OrgId} and event with id {EventId}", orgId, eventId);
                return Task.FromResult(Enumerable.Empty<FlowOverviewDto>());
            }
            return flowList;
        }

        public Task<bool> CreateAction(Guid orgId, Guid eventId, Guid flowId, ActionCreateDto actionDto)
        {
            throw new NotImplementedException();
        }

        public Task<FlowDto> CreateFlow(Guid orgId, Guid eventId, FlowCreateDto flowDto)
        {
            throw new NotImplementedException();
        }

        public Task<TriggerDto> CreateTrigger(Guid orgId, Guid eventId, Guid flowId, TriggerCreateDto triggerDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAction(Guid orgId, Guid eventId, Guid flowId, Guid actionId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFlow(Guid orgId, Guid eventId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTrigger(Guid orgId, Guid eventId, Guid flowId, Guid triggerId)
        {
            throw new NotImplementedException();
        }

        public Task<ActionDto> GetActionDetails(Guid orgId, Guid eventId, Guid flowId, Guid actionId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ActionDto>> GetActions(Guid orgId, Guid eventId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        public Task<FlowDto> GetFlowDetails(Guid orgId, Guid eventId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        public Task<TriggerDto> GetTriggerDetails(Guid orgId, Guid eventId, Guid flowId, Guid triggerId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TriggerDto>> GetTriggers(Guid orgId, Guid eventId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        public Task<ActionDto> UpdateAction(Guid orgId, Guid eventId, Guid flowId, Guid actionId, ActionUpdateDto actionDto)
        {
            throw new NotImplementedException();
        }

        public Task<FlowDto> UpdateFlow(Guid orgId, Guid eventId, Guid flowId, FlowUpdateDto flowDto)
        {
            throw new NotImplementedException();
        }

        public Task<TriggerDto> UpdateTrigger(Guid orgId, Guid eventId, Guid flowId, Guid triggerId, TriggerUpdateDto triggerDto)
        {
            throw new NotImplementedException();
        }
    }
}
