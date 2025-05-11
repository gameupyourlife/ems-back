using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Service
{
    public interface IEventFlowService
    {
        Task<IEnumerable<FlowOverviewDto>> GetAllFlows(Guid orgId, Guid eventId);
        Task<FlowDto> CreateFlow(Guid orgId, Guid eventId, FlowCreateDto flowDto);
        Task<FlowDto> GetFlowDetails(Guid orgId, Guid eventId, Guid flowId);
        Task<FlowDto> UpdateFlow(Guid orgId, Guid eventId, Guid flowId, FlowUpdateDto flowDto);
        Task<bool> DeleteFlow(Guid orgId, Guid eventId, Guid flowId);
        Task<IEnumerable<ActionDto>> GetActions(Guid orgId, Guid eventId, Guid flowId);
        Task<bool> CreateAction(Guid orgId, Guid eventId, Guid flowId, ActionCreateDto actionDto);
        Task<ActionDto> GetActionDetails(Guid orgId, Guid eventId, Guid flowId, Guid actionId);
        Task<ActionDto> UpdateAction(Guid orgId, Guid eventId, Guid flowId, Guid actionId, ActionUpdateDto actionDto);
        Task<bool> DeleteAction(Guid orgId, Guid eventId, Guid flowId, Guid actionId);
        Task<IEnumerable<TriggerDto>> GetTriggers(Guid orgId, Guid eventId, Guid flowId);
        Task<TriggerDto> CreateTrigger(Guid orgId, Guid eventId, Guid flowId, TriggerCreateDto triggerDto);
        Task<TriggerDto> GetTriggerDetails(Guid orgId, Guid eventId, Guid flowId, Guid triggerId);
        Task<TriggerDto> UpdateTrigger(Guid orgId, Guid eventId, Guid flowId, Guid triggerId, TriggerUpdateDto triggerDto);
        Task<bool> DeleteTrigger(Guid orgId, Guid eventId, Guid flowId, Guid triggerId);
    }
}
