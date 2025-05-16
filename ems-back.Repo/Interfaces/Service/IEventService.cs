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
    Task<IEnumerable<EventOverviewDto>> GetAllEventsAsync(Guid orgId);
    Task<EventInfoDto> CreateEventAsync(Guid orgId, EventCreateDto eventDto, Guid userId);
    Task<EventInfoDto> GetEventAsync(Guid orgId, Guid eventId);
    Task<EventInfoDto> UpdateEventAsync(Guid orgId, Guid eventId, EventUpdateDto eventDto);
    Task<bool> DeleteEventAsync(Guid orgId, Guid eventId);
    Task<IEnumerable<EventAttendeeDto>> GetAllEventAttendeesAsync(Guid orgId, Guid eventId);
    Task<EventAttendeeDto> AddAttendeeToEventAsync(Guid orgId, Guid eventId, EventAttendeeCreateDto attendeeDto);
    Task<bool> RemoveAttendeeFromEventAsync(Guid eventId, Guid userId, Guid attendeeId);
    Task<IEnumerable<AgendaEntryDto>> GetAgendaAsync(Guid orgId, Guid eventId);
    Task<AgendaEntryDto> AddAgendaPointToEventAsync(Guid orgId, Guid eventId, AgendaEntryCreateDto agendaEntryDto);
    Task<AgendaEntryDto> UpdateAgendaPointAsync(Guid orgId, Guid eventId, Guid agendaId, AgendaEntryDto agendaEntryDto);
    Task<bool> DeleteAgendaPointAsync(Guid orgId, Guid eventId, Guid agendaId);
    Task<IEnumerable<FileDto>> GetFilesFromEventAsync(Guid orgId, Guid eventId);
    Task<FileDto> AddFileToEventAsync(Guid orgId, Guid eventId, FileDto file);
    Task<FileDto> UpdateFileAsync(Guid orgId, Guid eventId, Guid fileId, FileDto file);
    Task<FileDto> DeleteFileAsync(Guid orgId, Guid eventId, Guid fileId);
}
