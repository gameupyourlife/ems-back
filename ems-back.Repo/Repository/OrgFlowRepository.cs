using AutoMapper;
using ems_back.Repo.Data;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.Flow.FlowTemplate;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Repository
{
    public class OrgFlowRepository : IOrgFlowRepository {

        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public OrgFlowRepository(ApplicationDbContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FlowTemplateResponseDto>> GetAllFlowTemplatesAsync(Guid orgId)
        {
            var flowTemplates = await _dbContext.FlowTemplates
                .Where(e => e.OrganizationId == orgId)
                .Select(e => new FlowTemplateResponseDto
                {
                    FlowTemplateId = e.FlowTemplateId,
                    Name = e.Name,
                    Description = e.Description,
                    OrganizationId = e.OrganizationId,
                    CreatedAt = e.CreatedAt,
                    CreatedBy = e.CreatedBy,
                    UpdatedAt = e.UpdatedAt,
                    UpdatedBy = e.UpdatedBy
                })
                .AsNoTracking()
                .ToListAsync();

            return flowTemplates;
        }

        public async Task<FlowTemplate> CreateFlowTemplateAsync(FlowTemplate flowTemplate)
        {
            if (flowTemplate == null)
            {
                throw new ArgumentNullException(nameof(flowTemplate));
            }

            // Füge das Template zur DB hinzu
            await _dbContext.FlowTemplates.AddAsync(flowTemplate);
            await _dbContext.SaveChangesAsync();

            return flowTemplate;
        }

        public async Task<FlowTemplateResponseDto> GetFlowTemplateByIdAsync(Guid orgId, Guid templateId)
        {
            var templateDto = await _dbContext.FlowTemplates
                .Where(t => t.OrganizationId == orgId && t.FlowTemplateId == templateId)
                .Select(t => new FlowTemplateResponseDto
                {
                    FlowTemplateId = t.FlowTemplateId,
                    Name = t.Name,
                    Description = t.Description,
                    OrganizationId = t.OrganizationId,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return templateDto;
        }

        public async Task<FlowTemplateResponseDto> UpdateFlowTemplateAsync(FlowTemplateResponseDto updatedTemplate)
        {
            var existing = await _dbContext.FlowTemplates.FindAsync(updatedTemplate.FlowTemplateId);
            if (existing == null)
                return null;

            // Werte übernehmen
            existing.Name = updatedTemplate.Name;
            existing.Description = updatedTemplate.Description;
            existing.UpdatedAt = updatedTemplate.UpdatedAt;
            existing.UpdatedBy = updatedTemplate.UpdatedBy;

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<FlowTemplateResponseDto>(existing);
        }

        public async Task<bool> DeleteFlowTemplateAsync(Guid orgId, Guid templateId)
        {
            var flowTemplate = await _dbContext.FlowTemplates
                .FirstOrDefaultAsync(f => f.OrganizationId == orgId && f.FlowTemplateId == templateId);

            if (flowTemplate == null)
                return false;

            _dbContext.FlowTemplates.Remove(flowTemplate);
            await _dbContext.SaveChangesAsync();

            return true;
        }

    }
}