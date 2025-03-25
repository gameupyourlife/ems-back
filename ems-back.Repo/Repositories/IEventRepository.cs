using ems_back.Repo.Models;

namespace ems_back.Repo.Repositories;

public interface IEventRepository
{
	Task<IEnumerable<Event>> GetAllEventsAsync();
	Task<Event> GetEventByIdAsync(int id);
	Task<IEnumerable<Event>> GetEventsByOrganizationIdAsync(int organizationId);
	Task<IEnumerable<Event>> GetEventsByUserIdAsync(int userId);
	Task AddEventAsync(Event eventEntity);
	Task UpdateEventAsync(Event eventEntity);
	Task DeleteEventAsync(int id);



}