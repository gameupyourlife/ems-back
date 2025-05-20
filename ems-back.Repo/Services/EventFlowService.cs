using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using ems_back.Repo.Repository;
using Microsoft.Extensions.Logging;

namespace ems_back.Repo.Services
{
    public class EventFlowService : IEventFlowService
    {
        private readonly IEventFlowRepository _eventFlowRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<EventFlowService> _logger;

        public EventFlowService(
            IEventFlowRepository eventFlowRepository,
            IEventRepository eventRepository,
            ILogger<EventFlowService> logger)
        {
            _eventFlowRepository = eventFlowRepository;
            _logger = logger;
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<FlowOverviewDto>> GetAllFlows(Guid orgId, Guid eventId)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(orgId, eventId);
            if (eventEntity == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                return null;
            }

            var flowList = await _eventFlowRepository.GetAllFlowsAsync(eventId);
            if (flowList == null)
            {
                _logger.LogWarning("No flows found for organization with id {OrgId} and event with id {EventId}", orgId, eventId);
                return null;
            }
            return flowList;
        }

        public async Task<FlowOverviewDto> CreateFlowAsync(Guid orgId, Guid eventId, FlowCreateDto flowCreateDto)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(orgId, eventId);
            if (eventEntity == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                return null;
            }

            if (flowCreateDto == null)
            {
                throw new ArgumentNullException(nameof(flowCreateDto));
            }

            try
            {
                _logger.LogInformation("Attempting to create flow with Name: {Name}", flowCreateDto.Name);

                // 1. Mapping CreateDto -> Entity
                var flow = new Flow
                {
                    FlowId = Guid.NewGuid(),
                    Name = flowCreateDto.Name,
                    Description = flowCreateDto.Description,
                    EventId = eventId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = flowCreateDto.CreatedBy,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = flowCreateDto.CreatedBy,
                    Triggers = new List<Trigger>(),
                    Actions = new List<Models.Action>()
                };

                // 2. Persistieren
                var createdFlow = await _eventFlowRepository.CreateFlowAsync(flow);

                _logger.LogInformation("Successfully created flow with ID: {FlowId}", createdFlow.FlowId);

                // 3. Mapping Entity -> OverviewDto
                return new FlowOverviewDto
                {
                    Id = createdFlow.FlowId,
                    Name = createdFlow.Name,
                    Description = createdFlow.Description,
                    Triggers = new List<TriggerOverviewDto>(),
                    Actions = new List<ActionOverviewDto>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating flow with name: {Name}", flowCreateDto.Name);
                throw;
            }
        }

        
        public async Task<FlowOverviewDto> GetFlowByIdAsync(Guid orgId, Guid eventId, Guid flowId)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(orgId, eventId);
            if (eventEntity == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                return null;
            }

            var flow = await _eventFlowRepository.GetFlowByIdAsync(eventId, flowId);
            if (flow == null) return null;

            return new FlowOverviewDto
            {
                Id = flow.Id,
                Name = flow.Name,
                Description = flow.Description,
                Triggers = flow.Triggers.Select(t => new TriggerOverviewDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Summary = t.Summary,
                    Type = t.Type
                }).ToList(),
                Actions = flow.Actions.Select(a => new ActionOverviewDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Summary = a.Summary,
                    Type = a.Type
                }).ToList()
            };
        }

        public async Task<FlowOverviewDto> UpdateFlow(Guid orgId, Guid eventId, Guid flowId, FlowUpdateDto flowDto)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(orgId, eventId);
            if (eventEntity == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                return null;
            }

            var flow = await _eventFlowRepository.GetFlowByIdAsync(eventId, flowId);
            if (flow == null) return null;

            flow.Name = flowDto.Name;
            flow.Description = flowDto.Description;
            flow.UpdatedAt = DateTime.UtcNow;
            flow.UpdatedBy = flowDto.UpdatedBy;

            var updated = await _eventFlowRepository.UpdateFlowAsync(flow);

            return updated;
        }

        public async Task<bool> DeleteFlow(Guid orgId, Guid eventId, Guid flowId)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(orgId, eventId);
            if (eventEntity == null)
            {
                _logger.LogWarning("Event with id {EventId} not found", eventId);
                return false;
            }

            var flow = await _eventFlowRepository.GetFlowByIdAsync(eventId, flowId);
            if (flow == null) return false;

            return await _eventFlowRepository.DeleteFlowAsync(eventId, flowId);
        }

        //actions
        public async Task<IEnumerable<ActionDto>> GetActionsForFlowAsync(Guid eventId, Guid flowId)
        {
            return await _eventFlowRepository.GetActionsForFlowAsync(eventId, flowId);
        }

        public async Task<ActionDto> CreateActionAsync(Guid eventId, Guid flowId, ActionCreateDto dto)
        {
            var newAction = new Models.Action
            {
                Id = Guid.NewGuid(),
                Type = dto.Type,
                Details = dto.Details ?? string.Empty, // raw JSON, ensure non-null value
                CreatedAt = DateTime.UtcNow,
                FlowId = flowId,
                Name = dto.Name,
                Summary = dto.Summary ?? string.Empty // ensure non-null value
            };

            var createdAction = await _eventFlowRepository.CreateActionAsync(newAction);
            return createdAction;
        }

        public async Task<ActionDto?> GetActionByIdAsync(Guid eventId, Guid flowId, Guid actionId)
        {
            var action = await _eventFlowRepository.GetActionByIdAsync(eventId, flowId, actionId);
            return action;
        }

        public async Task<ActionDto> UpdateActionAsync(Guid eventId, Guid flowId, Guid actionId, ActionUpdateDto dto)
        {
            // Prüfung, ob Action existiert im Repository (optional, falls im Repo erledigt)
            var existing = await _eventFlowRepository.GetActionByIdAsync(eventId, flowId, actionId);
            if (existing == null)
                throw new KeyNotFoundException("Action not found");

            // UpdateDto direkt ans Repository weitergeben
            var updatedAction = await _eventFlowRepository.UpdateActionAsync(actionId, dto);

            return updatedAction;
        }

        public async Task<bool> DeleteActionAsync(Guid eventId, Guid flowId, Guid actionId)
        {
            // Hole die Action inkl. Template & Org zum Check
            var action = await _eventFlowRepository.GetActionByIdAsync(eventId, flowId, actionId);

            if (action == null || action.FlowId != flowId)
            {
                return false;
            }

            await _eventFlowRepository.DeleteActionAsync(actionId);
            return true;
        }

        // Hier beginnt die Anpassung für Trigger
        //ab hier anpassen
        public async Task<IEnumerable<TriggerDto>> GetTriggersForFlowAsync(Guid orgId, Guid eventId, Guid flowId)
        {
            var triggers = await _eventFlowRepository.GetTriggersForFlowAsync(eventId, flowId);
            return triggers;
        }

        public async Task<TriggerDto> CreateTriggerAsync(Guid eventId, Guid flowId, TriggerCreateDto dto)
        {
            var newAction = new Trigger
            {
                Id = Guid.NewGuid(),
                Type = dto.Type,
                Details = dto.Details ?? string.Empty, // raw JSON, ensure non-null value
                CreatedAt = DateTime.UtcNow,
                FlowId = flowId,
                Name = dto.Name ?? string.Empty,
                Summary = dto.Summary ?? string.Empty // ensure non-null value
            };

            var createdTrigger = await _eventFlowRepository.CreateTriggerAsync(newAction);
            return createdTrigger;
        }

        public async Task<TriggerDto?> GetTriggerByIdAsync(Guid eventId, Guid flowId, Guid triggerId)
        {
            var trigger = await _eventFlowRepository.GetTriggerByIdAsync(eventId, flowId, triggerId);
            return trigger;
        }

        public async Task<TriggerDto> UpdateTriggerAsync(Guid eventId, Guid flowId, Guid triggerId, TriggerUpdateDto dto)
        {
            // Prüfung, ob Action existiert im Repository (optional, falls im Repo erledigt)
            var existing = await _eventFlowRepository.GetTriggerByIdAsync(eventId, flowId, triggerId);
            if (existing == null)
                throw new KeyNotFoundException("Trigger not found");

            // UpdateDto direkt ans Repository weitergeben
            var updatedTrigger = await _eventFlowRepository.UpdateTriggerAsync(triggerId, dto);

            return updatedTrigger;
        }

        public async Task<bool> DeleteTriggerAsync(Guid eventId, Guid flowId, Guid triggerId)
        {
            // Hole die Action inkl. Template & Org zum Check
            var trigger = await _eventFlowRepository.GetTriggerByIdAsync(eventId, flowId, triggerId);

            if (trigger == null || trigger.FlowId != flowId)
            {
                return false;
            }

            await _eventFlowRepository.DeleteTriggerAsync(triggerId);
            return true;
        }
    }
}
