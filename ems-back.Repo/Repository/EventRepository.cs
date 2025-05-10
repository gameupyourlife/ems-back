using ems_back.Repo.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Interfaces.Repository;
using System.Reflection;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.Models;
using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.Agenda;

namespace ems_back.Repo.Repository
{
    public class EventRepository : IEventRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public EventRepository(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

        public async Task<IEnumerable<EventOverviewDto>> GetAllEventsAsync(Guid orgId)
        {
            var events = await _context.Events
                .Where(e => e.OrganizationId == orgId)
                .Select(e => new EventOverviewDto
                {
                    Title = e.Title,
                    Category = e.Category,
                    Start = e.Start,
                    Location = e.Location,
                    Attendees = e.Attendees.Count,
                    Status = e.Status,
                    Description = e.Description
                })

                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<EventOverviewDto>>(events);
        }

        public async Task<EventInfoDto> GetEventByIdAsync(Guid orgId, Guid eventId)
		{
			var eventEntity = await _context.Events
				.Where(e => e.OrganizationId == orgId && e.Id == eventId)
				.Select( e => new EventInfoDto
                {
					
                    Id = e.Id,
                    Title = e.Title,
                    Category = e.Category,
                    Start = e.Start,
                    End = e.End,
                    Location = e.Location,
                    Description = e.Description,
                    Status = e.Status,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    CreatedBy = e.CreatedBy,
                    UpdatedBy = e.UpdatedBy
                    
                })
				.AsNoTracking()
                .FirstOrDefaultAsync();

            return eventEntity;
		}

        public async Task<List<EventAttendeeDto>> GetAllEventAttendeesAsync(Guid eventId)
        {
            var attendeesList = await _context.Events
                .Where(e => e.Id == eventId)
                .SelectMany(e => e.Attendees)
                .Select(a => new EventAttendeeDto
                {
                    UserId = a.UserId,
                    UserEmail = a.User.Email,
                    UserName = a.User.FirstName + " " + a.User.LastName,
                    Status = a.Status,
                    ProfilePicture = a.User.ProfilePicture,
                    RegisteredAt = a.RegisteredAt,
                })
                .ToListAsync();

            return attendeesList;
        }

        public async Task<IEnumerable<EventInfoDto>> GetUpcomingEventsAsync(int days = 30)
		{
			var cutoffDate = DateTime.UtcNow.AddDays(days);
			var events = await _context.Events
				.Where(e => e.Start >= DateTime.UtcNow && e.Start <= cutoffDate)
				.OrderBy(e => e.Start)
				.Include(e => e.Creator)
				.Include(e => e.Attendees)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventInfoDto>>(events);
		}

		public async Task<IEnumerable<EventInfoDto>> GetEventsByOrganizationAsync(Guid organizationId)
		{
			var events = await _context.Events
				.Where(e => e.OrganizationId == organizationId)
				.Include(e => e.Creator)
				.Include(e => e.Attendees)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventInfoDto>>(events);
		}

		public async Task<IEnumerable<EventInfoDto>> GetEventsByCreatorAsync(Guid userId)
		{
			var events = await _context.Events
				.Where(e => e.CreatedBy == userId)
				.Include(e => e.Updater)
				.Include(e => e.Attendees)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventInfoDto>>(events);
		}

		public async Task<IEnumerable<EventInfoDto>> GetEventsByCategoryAsync(int category)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<EventInfoDto>> GetEventsByDateRangeAsync(DateTime start, DateTime end)
		{
			var events = await _context.Events
				.Where(e => e.Start >= start && e.End <= end)
				.OrderBy(e => e.Start)
				.Include(e => e.Creator)
				.Include(e => e.Attendees)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventInfoDto>>(events);
		}

		public async Task<Guid?> CreateEventAsync(EventInfoDto eventDto)
		{
            if (eventDto == null)
            {
                return null;
            }
                
            var eventObject = new Event();
            _mapper.Map(eventDto, eventObject);
            _context.Events.Add(eventObject);
            await _context.SaveChangesAsync();
            return eventObject.Id;
        }

		public async Task<EventInfoDto> UpdateAsync(EventInfoDto eventDto)
		{
			var existingEvent = await _context.Events.FindAsync(eventDto.Id);
			if (existingEvent == null)
				return null;

			_mapper.Map(eventDto, existingEvent);
			existingEvent.UpdatedAt = DateTime.UtcNow;

			_context.Events.Update(existingEvent);
			await _context.SaveChangesAsync();

			return await GetEventByIdAsync(eventDto.OrganizationId, eventDto.Id);
		}

        public async Task<EventInfoDto> UpdateStatusAsync(Guid eventId, EventInfoDto statusDto)
        {
            var existingEvent = await _context.Events.FindAsync(eventId);
            if (existingEvent == null)
                return null;

            existingEvent.Status = statusDto.Status;
            existingEvent.UpdatedBy = statusDto.UpdatedBy;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            _context.Events.Update(existingEvent);
            await _context.SaveChangesAsync();

            return await GetEventByIdAsync(existingEvent.OrganizationId, eventId);
        }

		public async Task<bool> DeleteAsync(Guid id)
		{
			var eventEntity = await _context.Events.FindAsync(id);
			if (eventEntity == null)
				return false;

			_context.Events.Remove(eventEntity);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			return await _context.Events.AnyAsync(e => e.Id == id);
		}

		public async Task<List<AgendaEntryDto>> GetAgendaWithEventAsync(Guid orgId, Guid eventId)
		{
			var eventEntity = await _context.AgendaEntries
				.Where(e => e.EventId == eventId)
                .Select(e => new AgendaEntryDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Start = e.Start,
                    End = e.End,
                    EventId = eventId
                })
				.ToListAsync();

            return eventEntity;
		}

        public async Task<List<FileDto>> GetFilesFromEvent(Guid eventId)
        {
            var files = await _context.Files
                .Where(e => e.Event.Id == eventId)
                .Select(f => new FileDto
                {
                    Id = f.Id,
                    Url = f.Url,
                    Type = f.Type,
                    UploadedAt = f.UploadedAt,
                    OriginalName = f.Name,
                    SizeInBytes = f.SizeInBytes
                })
                .ToListAsync();

            return files;
        }

        public async Task<EventInfoDto> GetEventWithAllDetailsAsync(Guid eventId)
		{
			var eventEntity = await _context.Events
				.Include(e => e.Creator)
				.Include(e => e.Updater)
				.Include(e => e.Attendees)
					.ThenInclude(a => a.User)
				.FirstOrDefaultAsync(e => e.Id == eventId);

			return _mapper.Map<EventInfoDto>(eventEntity);
		}

		public async Task<int> GetAttendeeCountAsync(Guid eventId)
		{
			return await _context.EventAttendees
				.CountAsync(ea => ea.EventId == eventId);
		}

        Task<EventInfoDto> IEventRepository.UpdateEventAsync(EventInfoDto eventDto)
        {
            throw new NotImplementedException();
        }

        // Hilfsklassen EventService:

        public async Task<EventOverviewDto> GetEventByTitleAndDateAsync(string title, DateTime start, Guid orgId)
        {
            var eventEntity = await _context.Events
                .Where(e => e.Title == title && e.Start == start && e.OrganizationId == orgId)
                .Select(e => new EventOverviewDto
                {
                    Title = e.Title,
                    Category = e.Category,
                    Start = e.Start,
                    Location = e.Location,
                    Attendees = e.Attendees.Count,
                    Status = e.Status,
                    Description = e.Description
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(); 

            return eventEntity;
        }
    }
}