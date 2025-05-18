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
        // Flow
        Task<IEnumerable<FlowOverviewDto>> GetAllFlows(Guid orgId, Guid eventId);
        Task<FlowOverviewDto> CreateFlowAsync(Guid orgId, Guid eventId, FlowCreateDto flowCreateDto);
        Task<FlowOverviewDto> GetFlowByIdAsync(Guid orgId, Guid eventId, Guid flowId);
        Task<FlowResponseDto> UpdateFlow(Guid orgId, Guid eventId, Guid flowId, FlowUpdateDto flowDto);
        Task<bool> DeleteFlow(Guid orgId, Guid eventId, Guid flowId);

        // Actions
        Task<IEnumerable<ActionDto>> GetActionsForFlowAsync(Guid eventId, Guid flowId);
        Task<ActionDto> CreateActionAsync(Guid eventId, Guid flowId, ActionCreateDto dto);
        Task<ActionDto?> GetActionByIdAsync(Guid eventId, Guid flowId, Guid actionId);
        Task<ActionDto> UpdateActionAsync(Guid eventId, Guid flowId, Guid actionId, ActionUpdateDto dto);
        Task<bool> DeleteActionAsync(Guid eventId, Guid flowId, Guid actionId);

        // Triggers
        Task<IEnumerable<TriggerDto>> GetTriggersForFlowAsync(Guid orgId, Guid eventId, Guid flowId);
        Task<TriggerDto> CreateTriggerAsync(Guid eventId, Guid flowId, TriggerCreateDto dto);
        Task<TriggerDto?> GetTriggerByIdAsync(Guid eventId, Guid flowId, Guid triggerId);
        Task<TriggerDto> UpdateTriggerAsync(Guid eventId, Guid flowId, Guid triggerId, TriggerUpdateDto dto);
        Task<bool> DeleteTriggerAsync(Guid eventId, Guid flowId, Guid triggerId);
    }
}
