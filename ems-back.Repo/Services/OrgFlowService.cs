using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.Flow.FlowTemplate;
using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using ems_back.Repo.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

namespace ems_back.Repo.Services
{
    public class OrgFlowService : IOrgFlowService
    {
        private readonly IOrgFlowRepository _orgFlowRepository;
        private readonly ILogger<EventService> _logger;
        private readonly IMapper _mapper;

        public OrgFlowService(
            IOrgFlowRepository orgFlowRepository,
            ILogger<EventService> logger, 
            IMapper mapper)
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
                    UpdatedBy = flowTemplateCreateDto.CreatedBy,
                    Triggers = new List<Trigger>(),
                    Actions = new List<Models.Action>()
                };

                // 2. Persistieren
                var createdTemplate = await _orgFlowRepository.CreateFlowTemplateAsync(flowTemplate);

                _logger.LogInformation("Successfully created template with ID: {TemplateId}", createdTemplate.FlowTemplateId);

                // 3. Mapping Entity -> ResponseDto (initial leere Listen)
                return new FlowTemplateResponseDto
                {
                    Id = createdTemplate.FlowTemplateId,
                    Name = createdTemplate.Name,
                    Description = createdTemplate.Description,
                    OrganizationId = createdTemplate.OrganizationId,
                    CreatedAt = createdTemplate.CreatedAt,
                    UpdatedAt = createdTemplate.UpdatedAt,
                    CreatedBy = createdTemplate.CreatedBy,
                    UpdatedBy = createdTemplate.UpdatedBy,
                    Triggers = new List<TriggerOverviewDto>(),
                    Actions = new List<ActionOverviewDto>()
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
            if (template == null)
            {
                throw new KeyNotFoundException($"Flow template with ID {templateId} not found for organization {orgId}");
            }

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

        public async Task<IEnumerable<ActionDto>> GetActionsForTemplateAsync(Guid orgId, Guid templateId)
        {
            return await _orgFlowRepository.GetActionsForTemplateAsync(orgId, templateId);
        }

        public async Task<ActionDto> CreateActionAsync(Guid orgId, Guid templateId, ActionCreateDto dto)
        {
            var newAction = new Models.Action
            {
                Id = Guid.NewGuid(),
                Type = dto.Type,
                Details = dto.Details ?? string.Empty, // raw JSON, ensure non-null value
                CreatedAt = DateTime.UtcNow,
                FlowTemplateId = templateId,
                Name = dto.Name,
                Description = dto.Description ?? string.Empty // ensure non-null value
            };

            var createdAction = await _orgFlowRepository.CreateActionAsync(newAction);
            return createdAction;
        }

        public async Task<ActionDto?> GetActionByIdAsync(Guid orgId, Guid templateId, Guid actionId)
        {
            var action = await _orgFlowRepository.GetActionByIdAsync(orgId, templateId, actionId);
            if (action == null)
            {
                throw new KeyNotFoundException($"Action with ID {actionId} not found for template {templateId} in organization {orgId}");
            }
            return action;
        }

        public async Task<ActionDto> UpdateActionAsync(Guid orgId, Guid templateId, Guid actionId, ActionUpdateDto dto)
        {
            // Prüfung, ob Action existiert im Repository (optional, falls im Repo erledigt)
            var existing = await _orgFlowRepository.GetActionByIdAsync(orgId, templateId, actionId);
            if (existing == null)
                throw new KeyNotFoundException("Action not found");

            // UpdateDto direkt ans Repository weitergeben
            var updatedAction = await _orgFlowRepository.UpdateActionAsync(actionId, dto);

            return updatedAction;
        }

        public async Task<bool> DeleteActionAsync(Guid orgId, Guid templateId, Guid actionId)
        {
            // Hole die Action inkl. Template & Org zum Check
            var action = await _orgFlowRepository.GetActionByIdAsync(orgId, templateId, actionId);

            if (action == null || action.FlowTemplateId != templateId)
            {
                return false;
            }

            await _orgFlowRepository.DeleteActionAsync(actionId);
            return true;
        }

        // Hier beginnt die Anpassung für Trigger
        //ab hier anpassen
        public async Task<IEnumerable<TriggerDto>> GetTriggersForTemplateAsync(Guid orgId, Guid templateId)
        {
            var triggers = await _orgFlowRepository.GetTriggersForTemplateAsync(orgId, templateId);
            return triggers;
        }

        public async Task<TriggerDto> CreateTriggerAsync(Guid orgId, Guid templateId, TriggerCreateDto dto)
        {
            var newAction = new Trigger
            {
                Id = Guid.NewGuid(),
                Type = dto.Type,
                Details = dto.Details ?? string.Empty, // raw JSON, ensure non-null value
                CreatedAt = DateTime.UtcNow,
                FlowTemplateId = templateId,
                Name = dto.Name ?? string.Empty,
                Summary = dto.Description ?? string.Empty // ensure non-null value
            };

            var createdTrigger = await _orgFlowRepository.CreateTriggerAsync(newAction);
            return createdTrigger;
        }

        public async Task<TriggerDto?> GetTriggerByIdAsync(Guid orgId, Guid templateId, Guid triggerId)
        {
            var trigger = await _orgFlowRepository.GetTriggerByIdAsync(orgId, templateId, triggerId);
            if (trigger == null)
            {
                throw new KeyNotFoundException($"Trigger with ID {triggerId} not found for template {templateId} in organization {orgId}");
            }
            return trigger;
        }

        public async Task<TriggerDto> UpdateTriggersAsync(Guid orgId, Guid templateId, Guid triggerId, TriggerUpdateDto dto)
        {
            // Prüfung, ob Action existiert im Repository (optional, falls im Repo erledigt)
            var existing = await _orgFlowRepository.GetTriggerByIdAsync(orgId, templateId, triggerId);
            if (existing == null)
                throw new KeyNotFoundException("Trigger not found");

            // UpdateDto direkt ans Repository weitergeben
            var updatedTrigger = await _orgFlowRepository.UpdateTriggerAsync(triggerId, dto);

            return updatedTrigger;
        }

        public async Task<bool> DeleteTriggerAsync(Guid orgId, Guid templateId, Guid triggerId)
        {
            // Hole die Action inkl. Template & Org zum Check
            var trigger = await _orgFlowRepository.GetTriggerByIdAsync(orgId, templateId, triggerId);

            if (trigger == null || trigger.FlowTemplateId != templateId)
            {
                return false;
            }

            await _orgFlowRepository.DeleteTriggerAsync(triggerId);
            return true;
        }
    }
}
