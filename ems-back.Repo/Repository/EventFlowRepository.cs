using System.Text.Json;
using AutoMapper;
using ems_back.Repo.Data;
using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Flow.FlowTemplate;
using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Models;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Repository
{
    public class EventFlowRepository : IEventFlowRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public EventFlowRepository(ApplicationDbContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FlowOverviewDto>> GetAllFlowsAsync(Guid eventId)
        {
            var flows = await _dbContext.Flows
                .Where(f => f.EventId == eventId)
                .Include(f => f.Triggers)
                .Include(f => f.Actions)
                .AsNoTracking()
                .ToListAsync();

            var flowDtos = flows.Select(f => new FlowOverviewDto
            {
                Id = f.FlowId,
                Name = f.Name,
                Description = f.Description,
                Triggers = f.Triggers.Select(tr => new TriggerOverviewDto
                {
                    Id = tr.Id,
                    Name = tr.Name,
                    Summary = tr.Summary,
                    Type = tr.Type
                }).ToList(),
                Actions = f.Actions.Select(a => new ActionOverviewDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Summary = a.Summary,
                    Type = a.Type
                }).ToList()
            });

            return flowDtos;
        }


        public async Task<Flow> CreateFlowAsync(Flow flow)
        {
            if (flow == null)
            {
                throw new ArgumentNullException(nameof(flow));
            }

            await _dbContext.Flows.AddAsync(flow);
            await _dbContext.SaveChangesAsync();

            return flow;
        }

        public async Task<FlowOverviewDto?> GetFlowByIdAsync(Guid orgId, Guid eventId, Guid flowId)
        {
            var flowDto = await _dbContext.Flows
                .Where(f => f.EventId == eventId && f.FlowId == flowId)
                .Include(f => f.Triggers)
                .Include(f => f.Actions)
                .Select(f => new FlowOverviewDto
                {
                    Id = f.FlowId,
                    Name = f.Name,
                    Description = f.Description,
                    Triggers = f.Triggers.Select(t => new TriggerOverviewDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Summary = t.Summary,
                        Type = t.Type
                    }).ToList(),
                    Actions = f.Actions.Select(a => new ActionOverviewDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Summary = a.Summary,
                        Type = a.Type
                    }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return flowDto;
        }

        public async Task<FlowResponseDto> UpdateFlowAsync(FlowOverviewDto updatedFlow)
        {
            var existing = await _dbContext.Flows.FindAsync(updatedFlow.Id);
            if (existing == null)
                return null;

            // Werte übernehmen
            existing.Name = updatedFlow.Name;
            existing.Description = updatedFlow.Description;
            existing.UpdatedAt = updatedFlow.UpdatedAt;
            existing.UpdatedBy = updatedFlow.UpdatedBy;

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<FlowResponseDto>(existing);
        }


        public async Task<bool> DeleteFlowAsync(Guid eventId, Guid flowId)
        {
            var flow = await _dbContext.Flows
                .FirstOrDefaultAsync(f => f.EventId == eventId && f.FlowId == flowId);

            if (flow == null)
                return false;

            _dbContext.Flows.Remove(flow);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        //Actions und FLows Methoden
        public async Task<IEnumerable<ActionDto>> GetActionsForFlowAsync(Guid eventId, Guid flowId)
        {
            var actions = await _dbContext.Actions
                .Where(a => a.FlowId == flowId && a.Flow.EventId == eventId)
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
                FlowId = a.FlowId,
                FlowTemplateId = null,
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
                FlowId = action.FlowId,
                Name = action.Name,
                Summary = action.Summary
            };
        }

        public async Task<ActionDto?> GetActionByIdAsync(Guid orgId, Guid flowId, Guid actionId)
        {
            var action = await _dbContext.Actions
                .Where(a => a.FlowId == flowId && a.Id == actionId)
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
                FlowId = action.FlowId,
                FlowTemplateId = null,
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
                FlowId = existing.FlowId,
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
        public async Task<IEnumerable<TriggerDto>> GetTriggersForFlowAsync(Guid orgId, Guid flowId)
        {
            var triggers = await _dbContext.Triggers
                .Where(a => a.FlowId == flowId)
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
                FlowId = a.FlowId,
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
                FlowId = trigger.FlowId,
                Name = trigger.Name,
                Summary = trigger.Summary
            };
        }

        public async Task<TriggerDto?> GetTriggerByIdAsync(Guid orgId, Guid flowId, Guid triggerId)
        {
            var trigger = await _dbContext.Triggers
                .Where(t => t.FlowId == flowId && t.Id == triggerId)
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
                FlowId = trigger.FlowId,
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
                FlowId = existing.FlowId,
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