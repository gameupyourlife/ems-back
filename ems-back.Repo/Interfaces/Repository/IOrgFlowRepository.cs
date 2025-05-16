using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Flow.FlowTemplate;
using ems_back.Repo.Models;

namespace ems_back.Repo.Interfaces.Repository
{
    public interface IOrgFlowRepository
    {
        Task<IEnumerable<FlowTemplateResponseDto>> GetAllFlowTemplatesAsync(Guid orgId);
        Task<FlowTemplate> CreateFlowTemplateAsync(FlowTemplate flowTemplate);
        Task<FlowTemplateResponseDto> GetFlowTemplateByIdAsync(Guid orgId, Guid templateId);
        Task<FlowTemplateResponseDto> UpdateFlowTemplateAsync(FlowTemplateResponseDto updatedTemplate);
        Task<bool> DeleteFlowTemplateAsync(Guid orgId, Guid templateId);
    }
}
