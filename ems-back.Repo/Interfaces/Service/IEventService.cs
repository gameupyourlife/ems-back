using ems_back.Repo.DTOs.Agency;
using ems_back.Repo.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.User;

public interface IEventService
{
    Task<IEnumerable<EventInfoDTO>> GetAllEventsAsync();
    Task<IEnumerable<EventInfoDTO>> GetUpcomingEventsAsync(int days = 30);
    Task<EventInfoDTO> GetEventByIdAsync(Guid id);
    Task<EventInfoDTO> GetEventWithAttendeesAsync(Guid id);
    Task<EventInfoDTO> GetEventWithAgendaAsync(Guid id);
    Task<EventInfoDTO> GetEventWithAllDetailsAsync(Guid id);
    Task<IEnumerable<EventInfoDTO>> GetEventsByOrganizationAsync(Guid organizationId);
    Task<IEnumerable<EventInfoDTO>> GetEventsByCreatorAsync(Guid userId);
    Task<IEnumerable<EventInfoDTO>> GetEventsByCategoryAsync(int category);
    Task<IEnumerable<EventInfoDTO>> GetEventsByDateRangeAsync(DateTime start, DateTime end);
    Task<EventInfoDTO> CreateEventAsync(EventCreateDto eventDto);
    Task<bool> UpdateEventAsync(Guid id, EventInfoDTO eventDto);
    Task<EventInfoDTO> UpdateEventStatusAsync(Guid id, EventInfoDTO statusDto);
    Task<bool> DeleteEventAsync(Guid id);
    Task<int> GetAttendeeCountAsync(Guid id);
}
