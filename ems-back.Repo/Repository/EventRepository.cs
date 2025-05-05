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

		public async Task<EventDetailsDto> GetByIdAsync(Guid orgId, Guid eventId)
		{
			var eventEntity = await _context.Events
				.Where(e => e.OrganizationId == orgId && e.Id == eventId)
				.Select( e => new EventDetailsDto
				{
					Metadata = new EventInfoDTO
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
                    },
					Organization = new OrganizationOverviewDto
					{
                        Id = e.Organization.Id,
                        Name = e.Organization.Name,
						ProfilePicture = e.Organization.ProfilePicture,
                    },
					Attendees = e.Attendees.Select(a => new EventAttendeeDto
					{
						UserId = a.UserId,
						UserEmail = a.User.Email,
						UserName = a.User.FirstName + a.User.LastName,
						Status = a.Attended,
						ProfilePicture = a.User.ProfilePicture,
						RegisteredAt = a.RegisteredAt,
					})
                    .ToList(),
                })
                .FirstOrDefaultAsync();

			return eventEntity;
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

		public async Task<IEnumerable<EventInfoDTO>> GetUpcomingEventsAsync(int days = 30)
		{
			var cutoffDate = DateTime.UtcNow.AddDays(days);
			var events = await _context.Events
				.Where(e => e.Start >= DateTime.UtcNow && e.Start <= cutoffDate)
				.OrderBy(e => e.Start)
				.Include(e => e.Creator)
				.Include(e => e.Attendees)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventInfoDTO>>(events);
		}

		public async Task<IEnumerable<EventInfoDTO>> GetEventsByOrganizationAsync(Guid organizationId)
		{
			var events = await _context.Events
				.Where(e => e.OrganizationId == organizationId)
				.Include(e => e.Creator)
				.Include(e => e.Attendees)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventInfoDTO>>(events);
		}

		public async Task<IEnumerable<EventInfoDTO>> GetEventsByCreatorAsync(Guid userId)
		{
			var events = await _context.Events
				.Where(e => e.CreatedBy == userId)
				.Include(e => e.Updater)
				.Include(e => e.Attendees)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventInfoDTO>>(events);
		}

		public async Task<IEnumerable<EventInfoDTO>> GetEventsByCategoryAsync(int category)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<EventInfoDTO>> GetEventsByDateRangeAsync(DateTime start, DateTime end)
		{
			var events = await _context.Events
				.Where(e => e.Start >= start && e.End <= end)
				.OrderBy(e => e.Start)
				.Include(e => e.Creator)
				.Include(e => e.Attendees)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventInfoDTO>>(events);
		}

		public async Task<EventDetailsDto> AddAsync(EventCreateDto eventDto)
		{
			var eventEntity = _mapper.Map<Event>(eventDto);
			eventEntity.CreatedAt = DateTime.UtcNow;
			eventEntity.UpdatedAt = DateTime.UtcNow;

			await _context.Events.AddAsync(eventEntity);
			await _context.SaveChangesAsync();

			return await GetByIdAsync(eventEntity.Organization.Id, eventEntity.Id);
		}

		public async Task<EventDetailsDto> UpdateAsync(EventInfoDTO eventDto)
		{
			var existingEvent = await _context.Events.FindAsync(eventDto.Id);
			if (existingEvent == null)
				return null;

			_mapper.Map(eventDto, existingEvent);
			existingEvent.UpdatedAt = DateTime.UtcNow;

			_context.Events.Update(existingEvent);
			await _context.SaveChangesAsync();

			return await GetByIdAsync(eventDto.OrganizationId, eventDto.Id);
		}

        public async Task<EventDetailsDto> UpdateStatusAsync(Guid eventId, EventInfoDTO statusDto)
        {
            var existingEvent = await _context.Events.FindAsync(eventId);
            if (existingEvent == null)
                return null;

            existingEvent.Status = statusDto.Status;
            existingEvent.UpdatedBy = statusDto.UpdatedBy;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            _context.Events.Update(existingEvent);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(existingEvent.OrganizationId, eventId);
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

        public async Task<List<EventAttendeeDto>> GetEventAttendeesAsync(Guid eventId)
        {
            var attendeesList = await _context.Events
                .Where(e => e.Id == eventId)
                .SelectMany(e => e.Attendees)
                .Select(a => new EventAttendeeDto
                {
                    UserId = a.UserId,
                    UserEmail = a.User.Email,
                    UserName = a.User.FirstName + " " + a.User.LastName,
                    Status = a.Attended,
                    ProfilePicture = a.User.ProfilePicture,
                    RegisteredAt = a.RegisteredAt,
                })
                .ToListAsync();

            return attendeesList;
        }

		public async Task<List<AgendaEntry>> GetAgendaWithEventAsync(Guid eventId)
		{
			var eventEntity = await _context.EventAttendees
				.Where(e => e.EventId == eventId)
				.SelectMany(e => e.Event.AgendaItems)
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
                    OriginalName = f.OriginalName,
                    ContentType = f.ContentType,
                    SizeInBytes = f.SizeInBytes
                })
                .ToListAsync();

            return files;
        }

        public async Task<EventInfoDTO> GetEventWithAllDetailsAsync(Guid eventId)
		{
			var eventEntity = await _context.Events
				.Include(e => e.Creator)
				.Include(e => e.Updater)
				.Include(e => e.Attendees)
					.ThenInclude(a => a.User)
				.Include(e => e.AgendaItems)
				.FirstOrDefaultAsync(e => e.Id == eventId);

			return _mapper.Map<EventInfoDTO>(eventEntity);
		}

		public async Task<int> GetAttendeeCountAsync(Guid eventId)
		{
			return await _context.EventAttendees
				.CountAsync(ea => ea.EventId == eventId);
		}
	}
}