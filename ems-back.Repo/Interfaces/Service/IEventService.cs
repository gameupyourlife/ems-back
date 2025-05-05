using ems_back.Repo.DTOs.Agency;
using ems_back.Repo.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Models;

public interface IEventService
{
    Task<IEnumerable<EventOverviewDto>> GetAllEventsAsync(Guid orgId);
    Task<IEnumerable<EventInfoDTO>> GetUpcomingEventsAsync(int days = 30);
    Task<EventDetailsDto> GetEventByIdAsync(Guid orgId, Guid eventId);
    Task<List<EventAttendeeDto>> GetEventAttendeesAsync(Guid eventId);
    Task<List<AgendaEntry>> GetAgendaWithEventAsync(Guid id);
    Task<EventInfoDTO> GetEventWithAllDetailsAsync(Guid id);
    Task<IEnumerable<EventInfoDTO>> GetEventsByOrganizationAsync(Guid organizationId);
    Task<IEnumerable<EventInfoDTO>> GetEventsByCreatorAsync(Guid userId);
    Task<IEnumerable<EventInfoDTO>> GetEventsByCategoryAsync(int category);
    Task<IEnumerable<EventInfoDTO>> GetEventsByDateRangeAsync(DateTime start, DateTime end);
    Task<List<FileDto>> GetFilesFromEvent(Guid eventId);
    Task<EventDetailsDto> CreateEventAsync(EventCreateDto eventDto);
    Task<bool> UpdateEventAsync(Guid id, EventInfoDTO eventDto);
    Task<EventDetailsDto> UpdateEventStatusAsync(Guid id, EventInfoDTO statusDto);
    Task<bool> DeleteEventAsync(Guid id);
    Task<int> GetAttendeeCountAsync(Guid id);
}
