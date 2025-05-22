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
using ems_back.Repo.Exceptions;

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
                    Id = e.Id,
                    Title = e.Title,
                    Category = e.Category,
                    Start = e.Start,
                    Location = e.Location,
                    Image = e.Image,
                    AttendeeCount = e.AttendeeCount,
                    Capacity = e.Capacity,
                    Status = e.Status,
                    Description = e.Description
                })

                .AsNoTracking()
                .ToListAsync();

            return events;
        }

        public async Task<Guid> CreateEventAsync(EventInfoDto eventDto)
        {
            var eventObject = new Event
            {
                Title = eventDto.Title,
                Location = eventDto.Location,
                Description = eventDto.Description,
                Start = eventDto.Start,
                End = eventDto.End,
                Capacity = eventDto.Capacity,
                Category = eventDto.Category,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = eventDto.CreatedBy,
                OrganizationId = eventDto.OrganizationId,
                AttendeeCount = 0,
                Status = EventStatus.SCHEDULED,
            };
            _context.Events.Add(eventObject);
            await _context.SaveChangesAsync();
            return eventObject.Id;
        }

        public async Task<EventInfoDto> GetEventByIdAsync(Guid orgId, Guid eventId)
		{
            var eventEntity = await _context.Events
                .Where(e => e.OrganizationId == orgId && e.Id == eventId)

                .Select(e => new EventInfoDto
                {

                    Id = e.Id,
                    Title = e.Title,
                    OrganizationId = e.OrganizationId,
                    Category = e.Category,
                    Start = e.Start,
                    End = e.End,
                    Location = e.Location,
                    Description = e.Description,
                    Status = e.Status,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    CreatedBy = e.CreatedBy,
                    CreatorName = e.Creator.FullName,
                    UpdatedBy = e.UpdatedBy,
                    AttendeeCount = e.AttendeeCount,
                    Capacity = e.Capacity,
                    Image = e.Image,
                })
				.AsNoTracking()
                .FirstOrDefaultAsync();

            return eventEntity;
		}
    
        public async Task<EventInfoDto> UpdateEventAsync(Guid orgId, Guid eventId, EventUpdateDto eventDto, Guid userId)
        {

            var existingEvent = await _context.Events
                .Where(e => e.OrganizationId == orgId && e.Id == eventId)
                .FirstOrDefaultAsync();
            if (existingEvent == null)
            {
                throw new NotFoundException("Event not found");
            }
                
            _mapper.Map(eventDto, existingEvent);
            existingEvent.UpdatedAt = new DateTime(
                DateTime.UtcNow.Year,
                DateTime.UtcNow.Month,
                DateTime.UtcNow.Day,
                DateTime.UtcNow.Hour,
                DateTime.UtcNow.Minute,
                DateTime.UtcNow.Second,
                DateTimeKind.Utc
            );
            existingEvent.UpdatedBy = userId;

            _context.Events.Update(existingEvent);
            await _context.SaveChangesAsync();

            return await GetEventByIdAsync(orgId, eventId);
        }

        public async Task<bool> DeleteEventAsync(Guid orgId, Guid eventId)
        {

            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null || eventEntity.OrganizationId != orgId)
            {
                return false;
            }
                
            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EventAttendeeDto>> GetAllEventAttendeesAsync(Guid orgId, Guid eventId)
        {
            var attendeesList = await _context.Events
                .Where(e => e.OrganizationId == orgId && e.Id == eventId)
                .SelectMany(e => e.Attendees)
                .Select(a => new EventAttendeeDto
                {
                    UserId = a.UserId,
                    UserEmail = a.User.Email,
                    UserName = a.User.FirstName + " " + a.User.LastName,
                    Status = a.Status,
                    RegisteredAt = a.RegisteredAt,
                })
                .ToListAsync();

            return attendeesList;
        }

        public async Task<bool> AddAttendeeToEventAsync(EventAttendee attendee)
        {
            _context.EventAttendees.Add(attendee);
            if (await IncreaseAttendeeCount(attendee.EventId))
            {
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveAttendeeFromEventAsync(Guid eventId, Guid userId)
        {
            var attendee = await _context.EventAttendees.FindAsync(eventId, userId);
            if (attendee == null)
            {
                return false;
            }

            await DecreaseAttendeeCount(eventId);

            _context.EventAttendees.Remove(attendee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddEventOrganizerAsync(Guid orgId, Guid eventId, Guid organizerId)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null || eventEntity.OrganizationId != orgId)
            {
                return false;
            }
            var organizer = await _context.Users.FindAsync(organizerId);
            if (organizer == null)
            {
                return false;
            }

            var eventOrganizer = new EventOrganizer
            {
                EventId = eventId,
                UserId = organizerId
            };

            eventEntity.Organizers.Add(eventOrganizer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveEventOrganizerAsync(Guid orgId, Guid eventId, Guid organizerId)
        {
            var organizer = await _context.EventOrganizers.FindAsync(eventId, organizerId);
            if (organizer == null)
            {
                return false;
            }

            _context.EventOrganizers.Remove(organizer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AgendaEntryDto>> GetAgendaByEventIdAsync(Guid eventId)
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

        public async Task<Guid> AddAgendaEntryToEventAsync(AgendaEntryDto agendaEntry)
        {
            var entry = _mapper.Map<AgendaEntry>(agendaEntry);
            _context.AgendaEntries.Add(entry);
            await _context.SaveChangesAsync();

            return entry.Id;
        }

        public async Task<bool> UpdateAgendaEntryAsync(Guid agendaId, Guid eventId, AgendaEntryDto agendaEntry)
        {
            var existingEntry = await _context.AgendaEntries
                .FirstOrDefaultAsync(e => e.Id == agendaId && e.EventId == eventId);

            if (existingEntry == null)
            {
                return false;
            }

            existingEntry.Title = agendaEntry.Title;
            existingEntry.Description = agendaEntry.Description;
            existingEntry.Start = agendaEntry.Start;
            existingEntry.End = agendaEntry.End;

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteAgendaEntryAsync(Guid agendaId)
        {
            var entry = await _context.AgendaEntries.FindAsync(agendaId);
            if (entry == null)
            {
                return false;
            }

            _context.AgendaEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return true;
        }

        // Additional Methods:

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

        public async Task<EventInfoDto> UpdateEventStatusAsync(Guid eventId, EventInfoDto statusDto)
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

		public async Task<int> GetAttendeeCountAsync(Guid eventId)
		{
			return await _context.EventAttendees
				.CountAsync(ea => ea.EventId == eventId);
		}

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
                    AttendeeCount = e.Attendees.Count,
                    Capacity = e.Capacity,
                    Status = e.Status,
                    Description = e.Description
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(); 

            return eventEntity;
        }

        private async Task<bool> IncreaseAttendeeCount(Guid eventId)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null)
            {
                return false;
            }
            eventEntity.AttendeeCount++;
            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> DecreaseAttendeeCount(Guid eventId)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null)
            {
                return false;
            }
            eventEntity.AttendeeCount--;
            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EventAttendeeDto> GetEventAttendeeByIdAsync(Guid eventId, Guid userId)
        {
            var attendee = await _context.EventAttendees
                .Where(ea => ea.EventId == eventId && ea.UserId == userId)
                .Select(ea => new EventAttendeeDto
                {
                    UserId = ea.UserId,
                    UserEmail = ea.User.Email,
                    UserName = ea.User.FirstName + " " + ea.User.LastName,
                    Status = ea.Status,
                    RegisteredAt = ea.RegisteredAt,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return attendee;
        }

        public Task<EventOrganizer> GetEventOrganizerAsync(Guid eventId, Guid organizerId)
        {
            var organizer = _context.EventOrganizers
                .Where(eo => eo.EventId == eventId && eo.UserId == organizerId)
                .Include(eo => eo.User)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return organizer;
        }

        public async Task<AgendaEntry> GetAgendaEntryByIdAsync(Guid agendaId)
        {
            return await _context.AgendaEntries.FindAsync(agendaId);
        }

        public async Task<IEnumerable<EventOverviewDto>> GetAllEventsByCreatorAsync(Guid orgId, Guid creatorId)
        {
            var events = await _context.Events
                .Where(e => e.OrganizationId == orgId && e.CreatedBy == creatorId)
                .Select(e => new EventOverviewDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Category = e.Category,
                    Start = e.Start,
                    Location = e.Location,
                    Image = e.Image,
                    AttendeeCount = e.Attendees.Count,
                    Capacity = e.Capacity,
                    Status = e.Status,
                    Description = e.Description
                })

                .AsNoTracking()
                .ToListAsync();

            return events;
        }
    }
}