using ems_back.Repo.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Models;
using ems_back.Repo.DTOs.Agenda;

public interface IEventService
{
    Task<IEnumerable<EventOverviewDto>> GetAllEventsAsync(Guid orgId, Guid userId);
    Task<EventInfoDto> CreateEventAsync(Guid orgId, EventCreateDto eventDto, Guid userId);
    Task<EventInfoDto> GetEventAsync(Guid orgId, Guid eventId, Guid userId);
    Task<EventInfoDto> UpdateEventAsync(Guid orgId, Guid eventId, EventUpdateDto eventDto, Guid userId);
    Task<bool> DeleteEventAsync(Guid orgId, Guid eventId, Guid userId);
    Task<IEnumerable<EventAttendeeDto>> GetAllEventAttendeesAsync(Guid orgId, Guid eventId, Guid userId);
    Task<EventAttendeeDto> AddAttendeeToEventAsync(Guid orgId, Guid eventId, EventAttendeeCreateDto attendeeDto, Guid userId);
    Task<bool> RemoveAttendeeFromEventAsync(Guid orgId, Guid eventId, Guid attendeeId, Guid userId);
    Task<bool> AddEventOrganizerAsync(Guid orgId, Guid eventId, Guid organizerId, Guid userId);
    Task<bool> RemoveEventOrganizerAsync(Guid orgId, Guid eventId, Guid organizerId, Guid userId);
    Task<IEnumerable<AgendaEntryDto>> GetAgendaAsync(Guid orgId, Guid eventId, Guid userId);
    Task<AgendaEntryDto> AddAgendaEntryToEventAsync(Guid orgId, Guid eventId, AgendaEntryCreateDto agendaEntryDto, Guid userId);
    Task<AgendaEntryDto> UpdateAgendaEntryAsync(Guid orgId, Guid eventId, Guid agendaId, AgendaEntryCreateDto agendaEntryDto, Guid userId);
    Task<bool> DeleteAgendaEntryAsync(Guid orgId, Guid eventId, Guid agendaId, Guid userId);

    // helper:

    Task<bool> ExistsOrg(Guid orgId);
}
