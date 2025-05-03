using ems_back.Repo.DTOs.Agency;
using ems_back.Repo.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.User;

public interface IEventService
{
    // Events
    Task<IEnumerable<EventBasicDto>> GetEventsAsync(Guid orgId);
    Task<EventBasicDto> CreateEventAsync(Guid orgId, EventBasicDto eventDto);
    Task<EventBasicDto> GetEventByIdAsync(Guid orgId, Guid eventId);
    Task<EventBasicDto> UpdateEventAsync(Guid orgId, Guid eventId, EventBasicDto eventDto);
    Task DeleteEventAsync(Guid orgId, Guid eventId);

    // Attendees
    Task<IEnumerable<UserDto>> GetAttendeesAsync(Guid orgId, Guid eventId);
    Task<UserDto> AddAttendeeAsync(Guid orgId, Guid eventId, UserDto attendeeDto);
    Task RemoveAttendeeAsync(Guid orgId, Guid eventId, Guid userId);

    // Files
    Task<IEnumerable<FileDto>> GetFilesAsync(Guid orgId, Guid eventId);
    Task<FileDto> AddFileAsync(Guid orgId, Guid eventId, FileDto fileDto);
    Task<FileDto> UpdateFileAsync(Guid orgId, Guid eventId, Guid fileId, FileDto fileDto);
    Task DeleteFileAsync(Guid orgId, Guid eventId, Guid fileId);

    // Agenda
    Task<IEnumerable<AgendaEntryDto>> GetAgendaAsync(Guid orgId, Guid eventId);
    Task<AgendaEntryDto> AddAgendaEntryAsync(Guid orgId, Guid eventId, AgendaEntryDto agendaEntryDto);
    Task<AgendaEntryDto> UpdateAgendaEntryAsync(Guid orgId, Guid eventId, Guid agendaId, AgendaEntryDto agendaEntryDto);
    Task DeleteAgendaEntryAsync(Guid orgId, Guid eventId, Guid agendaId);
}
