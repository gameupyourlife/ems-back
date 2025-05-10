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

        public async Task<EventInfoDto> GetEventByIdAsync(Guid orgId, Guid eventId)
		{
			var eventEntity = await _context.Events
				.Where(e => e.OrganizationId == orgId && e.Id == eventId)
				.Select( e => new EventInfoDto
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
                    UpdatedBy = e.UpdatedBy
                    
                })
				.AsNoTracking()
                .FirstOrDefaultAsync();

            return eventEntity;
		}

        public async Task<EventInfoDto> UpdateEventAsync(Guid orgId, Guid eventId, EventInfoDto eventDto)
        {

            // To Do: Manuelle Überprüfung

            var existingEvent = await _context.Events.FindAsync(eventDto.Id);
            if (existingEvent == null)
                return null;

            _mapper.Map(eventDto, existingEvent);
            existingEvent.UpdatedAt = DateTime.UtcNow;

            _context.Events.Update(existingEvent);
            await _context.SaveChangesAsync();

            return await GetEventByIdAsync(eventDto.OrganizationId, eventDto.Id);
        }

        public async Task<bool> DeleteEventAsync(Guid orgId, Guid eventId)
        {

            // To Do: Manuelle Überprüfung

            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null)
                return false;

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
                    ProfilePicture = a.User.ProfilePicture,
                    RegisteredAt = a.RegisteredAt,
                })
                .ToListAsync();

            return attendeesList;
        }

        public async Task<EventAttendeeDto> AddAttendeeToEventAsync(Guid orgId, Guid eventId, EventAttendeeDto attendee)
        {
            throw new NotImplementedException("AddAttendeeToEventsAsync is not implemented yet");
        }

        public async Task<bool> RemoveAttendeeFromEventAsync(Guid orgId, Guid eventId, Guid userId)
        {
            throw new NotImplementedException("RemoveAttendeeFromEventAsync is not implemented yet");
        }

        public async Task<IEnumerable<AgendaEntryDto>> GetAgendaByEventIdAsync(Guid orgId, Guid eventId)
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

        public async Task<AgendaEntryDto> AddAgendaPointToEventAsync(Guid orgId, Guid eventId, AgendaEntryDto agendaEntry)
        {
            var agenda = new AgendaEntry
            {
                Title = agendaEntry.Title,
                Description = agendaEntry.Description,
                Start = agendaEntry.Start,
                End = agendaEntry.End,
                EventId = eventId
            };
            _context.AgendaEntries.Add(agenda);
            await _context.SaveChangesAsync();
            return _mapper.Map<AgendaEntryDto>(agenda);
        }

        public async Task<AgendaEntryDto> UpdateAgendaPointAsync(Guid orgId, Guid eventId, Guid agendaId, AgendaEntryDto agendaEntry)
        {
            throw new NotImplementedException("UpdateAgendaPointAsync is not implemented yet");
        }

        public async Task<AgendaEntryDto> DeleteAgendaPointAsync(Guid orgId, Guid eventId, Guid agendaId)
        {
            throw new NotImplementedException("DeleteAgendaPointAsync is not implemented yet");
        }

        public async Task<IEnumerable<FileDto>> GetFilesFromEvent(Guid orgId, Guid eventId)
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

        public async Task<FileDto> AddFileToEvent(Guid orgId, Guid eventId, FileDto file)
        {
            throw new NotImplementedException("AddFileToEvent is not implemented yet");
        }

        public async Task<FileDto> UpdateFile(Guid orgId, Guid eventId, Guid fileId, FileDto file)
        {
            throw new NotImplementedException("UpdateFile is not implemented yet");
        }

        public async Task<FileDto> RemoveFileFromEvent(Guid orgId, Guid eventId, Guid fileId)
        {
            throw new NotImplementedException("RemoveFileFromEvent is not implemented yet");
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