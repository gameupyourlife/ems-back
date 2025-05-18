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
using ems_back.Repo.DTOs.Action;
using System.Text.Json;
using ems_back.Repo.DTOs.Trigger;

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

        public async Task<IEnumerable<ActionDto>> GetActionsForTemplateAsync(Guid orgId, Guid templateId)
        {
            var actions = await _dbContext.Actions
                .Where(a => a.FlowTemplateId == templateId && a.FlowTemplate.OrganizationId == orgId)
                .AsNoTracking()
                .ToListAsync();

            var actionDtos = actions.Select(a => new ActionDto
            {
                Id = a.Id,
                Type = a.Type,
                Details = string.IsNullOrWhiteSpace(a.Details)
                    ? JsonDocument.Parse("{}").RootElement
                    : JsonDocument.Parse(a.Details).RootElement,
                CreatedAt = a.CreatedAt,
                FlowTemplateId = a.FlowTemplateId,
                Name = a.Name,
                Summary = a.Summary
            });

            return actionDtos;
        }

        public async Task<ActionDto> CreateActionAsync(Models.Action action)
        {
            _dbContext.Actions.Add(action);
            await _dbContext.SaveChangesAsync();

            return new ActionDto
            {
                Id = action.Id,
                Type = action.Type,
                Details = string.IsNullOrWhiteSpace(action.Details)
                    ? JsonDocument.Parse("{}").RootElement
                    : JsonDocument.Parse(action.Details).RootElement,
                CreatedAt = action.CreatedAt,
                FlowTemplateId = action.FlowTemplateId,
                Name = action.Name,
                Summary = action.Summary
            };
        }

        public async Task<ActionDto?> GetActionByIdAsync(Guid orgId, Guid templateId, Guid actionId)
        {
            var action = await _dbContext.Actions
                .Where(a => a.FlowTemplateId == templateId && a.Id == actionId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (action == null) return null;

            var detailsJson = string.IsNullOrWhiteSpace(action.Details)
                ? JsonDocument.Parse("{}").RootElement
                : JsonDocument.Parse(action.Details).RootElement;

            var actionDto = new ActionDto
            {
                Id = action.Id,
                Type = action.Type,
                Details = detailsJson,
                CreatedAt = action.CreatedAt,
                FlowTemplateId = action.FlowTemplateId,
                Name = action.Name,
                Summary = action.Summary
            };

            return actionDto;
        }

        public async Task<ActionDto> UpdateActionAsync(Guid actionId, ActionUpdateDto dto)
        {
            var existing = await _dbContext.Actions.FindAsync(actionId);
            if (existing == null)
                throw new KeyNotFoundException("Action not found");

            existing.Type = dto.Type;
            existing.Details = dto.Details;  // JsonElement -> JSON-String
            existing.Name = dto.Name;
            existing.Summary = dto.Summary;

            await _dbContext.SaveChangesAsync();

            return new ActionDto
            {
                Id = existing.Id,
                Type = existing.Type,
                Details = JsonDocument.Parse(existing.Details).RootElement,
                CreatedAt = existing.CreatedAt,
                FlowTemplateId = existing.FlowTemplateId,
                Name = existing.Name,
                Summary = existing.Summary
            };
        }

        public async Task<bool> DeleteActionAsync(Guid actionId)
        {
            var action = await _dbContext.Actions.FindAsync(actionId);
            if (action != null)
            {
                _dbContext.Actions.Remove(action);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        //Hier beginnt für Triggers
        //anpassen
        public async Task<IEnumerable<TriggerDto>> GetTriggersForTemplateAsync(Guid orgId, Guid templateId)
        {
            var triggers = await _dbContext.Triggers
                .Where(a => a.FlowTemplateId == templateId && a.FlowTemplate.OrganizationId == orgId)
                .AsNoTracking()
                .ToListAsync();

            var triggersDto = triggers.Select(a => new TriggerDto
            {
                Id = a.Id,
                Type = a.Type,
                Details = string.IsNullOrWhiteSpace(a.Details)
                    ? JsonDocument.Parse("{}").RootElement
                    : JsonDocument.Parse(a.Details).RootElement,
                CreatedAt = a.CreatedAt,
                FlowTemplateId = a.FlowTemplateId,
                Name = a.Name,
                Summary = a.Summary
            });

            return triggersDto;
        }

        public async Task<TriggerDto> CreateTriggerAsync(Trigger trigger)
        {
            _dbContext.Triggers.Add(trigger);
            await _dbContext.SaveChangesAsync();

            return new TriggerDto
            {
                Id = trigger.Id,
                Type = trigger.Type,
                Details = string.IsNullOrWhiteSpace(trigger.Details)
                    ? JsonDocument.Parse("{}").RootElement
                    : JsonDocument.Parse(trigger.Details).RootElement,
                CreatedAt = trigger.CreatedAt,
                FlowTemplateId = trigger.FlowTemplateId,
                Name = trigger.Name,
                Summary = trigger.Summary
            };
        }

        public async Task<TriggerDto?> GetTriggerByIdAsync(Guid orgId, Guid templateId, Guid triggerId)
        {
            var trigger = await _dbContext.Triggers
                .Where(t => t.FlowTemplateId == templateId && t.Id == triggerId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (trigger == null) return null;

            var detailsJson = string.IsNullOrWhiteSpace(trigger.Details)
                ? JsonDocument.Parse("{}").RootElement
                : JsonDocument.Parse(trigger.Details).RootElement;

            var triggerDto = new TriggerDto
            {
                Id = trigger.Id,
                Type = trigger.Type,
                Details = detailsJson,
                CreatedAt = trigger.CreatedAt,
                FlowTemplateId = trigger.FlowTemplateId,
                Name = trigger.Name,
                Summary = trigger.Summary
            };

            return triggerDto;
        }

        public async Task<TriggerDto> UpdateTriggerAsync(Guid triggerId, TriggerUpdateDto dto)
        {
            var existing = await _dbContext.Triggers.FindAsync(triggerId);
            if (existing == null)
                throw new KeyNotFoundException("Trigger not found");

            existing.Type = dto.Type;
            existing.Details = dto.Details ?? "{}"; // Ensure Details is not null
            existing.Name = dto.Name ?? existing.Name; // Fix for CS8601: Use existing.Name if dto.Name is null
            existing.Summary = dto.Summary ?? existing.Summary; // Fix for CS8601: Use existing.Summary if dto.Summary is null

            await _dbContext.SaveChangesAsync();

            return new TriggerDto
            {
                Id = existing.Id,
                Type = existing.Type,
                Details = JsonDocument.Parse(existing.Details).RootElement,
                CreatedAt = existing.CreatedAt,
                FlowTemplateId = existing.FlowTemplateId,
                Name = existing.Name,
                Summary = existing.Summary
            };
        }

        public async Task<bool> DeleteTriggerAsync(Guid triggerId)
        {
            var trigger = await _dbContext.Triggers.FindAsync(triggerId);
            if (trigger != null)
            {
                _dbContext.Triggers.Remove(trigger);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}