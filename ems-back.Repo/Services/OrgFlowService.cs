using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.Flow.FlowTemplate;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using ems_back.Repo.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ems_back.Repo.Services
{
    public class OrgFlowService : IOrgFlowService
    {
        private readonly IOrgFlowRepository _orgFlowRepository;
        private readonly ILogger<EventService> _logger;
        private IOrgFlowRepository @object;
        private readonly IMapper _mapper;

        public OrgFlowService(
            IOrgFlowRepository orgFlowRepository,
            ILogger<EventService> logger, IMapper mapper)
        {
            _orgFlowRepository = orgFlowRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FlowTemplateResponseDto>> GetAllFlowTemplatesAsync(Guid orgId)
        {
            if (!Guid.TryParse(orgId.ToString(), out Guid parsedOrgId))
            {
                _logger.LogWarning("Invalid organization ID format: {OrgId}", orgId);
                return Enumerable.Empty<FlowTemplateResponseDto>();
            }

            var flowTemplates = await _orgFlowRepository.GetAllFlowTemplatesAsync(orgId);

            if (flowTemplates == null)
            {
                _logger.LogWarning("No flow templates found for organization with id {OrgId}", orgId);
                return Enumerable.Empty<FlowTemplateResponseDto>();
            }
            else
            {
                return flowTemplates;
            }
        }

        public async Task<FlowTemplateResponseDto> CreateFlowTemplateAsync(Guid orgId, FlowTemplateCreateDto flowTemplateCreateDto)
        {
            if (flowTemplateCreateDto == null)
            {
                throw new ArgumentNullException(nameof(flowTemplateCreateDto));
            }

            try
            {
                _logger.LogInformation("Attempting to create flow with Name: {Name}", flowTemplateCreateDto.Name);

                // 1. Mapping CreateDto -> Entity
                var flowTemplate = new FlowTemplate
                {
                    FlowTemplateId = Guid.NewGuid(),
                    Name = flowTemplateCreateDto.Name,
                    Description = flowTemplateCreateDto.Description,
                    OrganizationId = orgId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = flowTemplateCreateDto.CreatedBy,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = flowTemplateCreateDto.CreatedBy
                };

                // 2. Persistieren
                var createdTemplate = await _orgFlowRepository.CreateFlowTemplateAsync(flowTemplate);

                _logger.LogInformation("Successfully created template with ID: {TemplateId}", createdTemplate.FlowTemplateId);

                // 3. Mapping Entity -> ResponseDto
                return new FlowTemplateResponseDto
                {
                    FlowTemplateId = createdTemplate.FlowTemplateId,
                    Name = createdTemplate.Name,
                    Description = createdTemplate.Description,
                    OrganizationId = createdTemplate.OrganizationId,
                    CreatedAt = createdTemplate.CreatedAt,
                    UpdatedAt = createdTemplate.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating flow template with name: {Name}", flowTemplateCreateDto.Name);
                throw;
            }
        }

        public async Task<FlowTemplateResponseDto?> GetFlowTemplateByIdAsync(Guid orgId, Guid templateId)
        {
            var template = await _orgFlowRepository.GetFlowTemplateByIdAsync(orgId, templateId);
            return template != null ? _mapper.Map<FlowTemplateResponseDto>(template) : null;
        }

        public async Task<FlowTemplateResponseDto?> UpdateFlowTemplateAsync(Guid orgId, Guid templateId, FlowTemplateUpdateDto dto)
        {
            var template = await _orgFlowRepository.GetFlowTemplateByIdAsync(orgId, templateId);
            if (template == null)
                return null;

            template.Name = dto.Name;
            template.Description = dto.Description;
            template.UpdatedAt = DateTime.UtcNow;
            template.UpdatedBy = dto.UpdatedBy;

            await _orgFlowRepository.UpdateFlowTemplateAsync(template);

            // Nur die Felder zurückgeben, die im ResponseDto definiert sind
            return _mapper.Map<FlowTemplateResponseDto>(template);
        }

        public async Task<bool> DeleteFlowTemplateAsync(Guid orgId, Guid templateId)
        {
            try
            {
                var flowTemplate = await _orgFlowRepository.GetFlowTemplateByIdAsync(orgId, templateId);
                if (flowTemplate == null)
                {
                    _logger.LogWarning("No flow template found with ID: {TemplateId} in organization {OrgId}", templateId, orgId);
                    return false;
                }

                var success = await _orgFlowRepository.DeleteFlowTemplateAsync(orgId, templateId);
                if (!success)
                {
                    _logger.LogWarning("Failed to delete flow template with ID: {TemplateId}", templateId);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting flow template with ID: {TemplateId}", templateId);
                throw;
            }
        }


    }
}
