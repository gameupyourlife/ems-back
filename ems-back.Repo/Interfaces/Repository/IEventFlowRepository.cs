using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Flow.FlowTemplate;
using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Repository
{
    public interface IEventFlowRepository
    {
        Task<IEnumerable<FlowOverviewDto>> GetAllFlowsAsync(Guid eventId);
        Task<Flow> CreateFlowAsync(Flow flow);
        Task<FlowOverviewDto?> GetFlowByIdAsync(Guid orgId, Guid eventId, Guid flowId);
        Task<FlowResponseDto> UpdateFlowAsync(FlowOverviewDto updatedFlow);
        Task<bool> DeleteFlowAsync(Guid eventId, Guid flowId);

        // Actions
        Task<IEnumerable<ActionDto>> GetActionsForFlowAsync(Guid orgId, Guid flowId);
        Task<ActionDto> CreateActionAsync(Models.Action action);
        Task<ActionDto?> GetActionByIdAsync(Guid orgId, Guid flowId, Guid actionId);
        Task<ActionDto> UpdateActionAsync(Guid actionId, ActionUpdateDto dto);
        Task<bool> DeleteActionAsync(Guid actionId);

        // Triggers
        Task<IEnumerable<TriggerDto>> GetTriggersForFlowAsync(Guid orgId, Guid flowId);
        Task<TriggerDto> CreateTriggerAsync(Trigger trigger);
        Task<TriggerDto?> GetTriggerByIdAsync(Guid orgId, Guid flowId, Guid triggerId);
        Task<TriggerDto> UpdateTriggerAsync(Guid triggerId, TriggerUpdateDto dto);
        Task<bool> DeleteTriggerAsync(Guid triggerId);
    }
}
