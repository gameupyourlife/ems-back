using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Flow.FlowTemplate;
using ems_back.Repo.DTOs.Trigger;

namespace ems_back.Repo.Interfaces.Service
{
    public interface IOrgFlowService
    {
        Task<IEnumerable<FlowTemplateResponseDto>> GetAllFlowTemplatesAsync(Guid orgId);
        Task<FlowTemplateResponseDto> CreateFlowTemplateAsync(Guid orgId, FlowTemplateCreateDto flowTemplateCreateDto);
        Task<FlowTemplateResponseDto?> GetFlowTemplateByIdAsync(Guid orgId, Guid templateId);
        Task<FlowTemplateResponseDto?> UpdateFlowTemplateAsync(Guid orgId, Guid templateId, FlowTemplateUpdateDto dto);
        Task<bool> DeleteFlowTemplateAsync(Guid orgId, Guid templateId);
        Task<IEnumerable<ActionDto>> GetActionsForTemplateAsync(Guid orgId, Guid templateId);
        Task<ActionDto> CreateActionAsync(Guid orgId, Guid templateId, ActionCreateDto dto);
        Task<ActionDto?> GetActionByIdAsync(Guid orgId, Guid templateId, Guid actionId);
        Task<ActionDto> UpdateActionAsync(Guid orgId, Guid templateId, Guid actionId, ActionUpdateDto dto);
        Task<bool> DeleteActionAsync(Guid orgId, Guid templateId, Guid actionId);
        Task<IEnumerable<TriggerDto>> GetTriggersForTemplateAsync(Guid orgId, Guid templateId);
        Task<TriggerDto?> GetTriggerByIdAsync(Guid orgId, Guid templateId, Guid triggerId);
        Task<TriggerDto> CreateTriggerAsync(Guid orgId, Guid templateId, TriggerCreateDto dto);
        Task<TriggerDto> UpdateTriggersAsync(Guid orgId, Guid templateId, Guid triggerId, TriggerUpdateDto dto);
        Task<bool> DeleteTriggerAsync(Guid orgId, Guid templateId, Guid triggerId);
    }
}
