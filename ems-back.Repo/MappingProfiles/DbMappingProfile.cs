using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ems_back.Repo.DTOs;
using ems_back.Repo.Models;
using Action = ems_back.Repo.Models.Action;

namespace ems_back.Repo.MappingProfiles
{
	public class DbMappingProfile : Profile
	{
		public DbMappingProfile()
		{
			// User mappings
			CreateMap<User, UserDto>()
				.ForMember(dest => dest.FullName,
					opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

			CreateMap<UserCreateDto, User>()
				.ForMember(dest => dest.UserName,
					opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.EmailConfirmed,
					opt => opt.MapFrom(src => false))
				.ForMember(dest => dest.Role,
					opt => opt.MapFrom(src => UserRole.Participant)); // Default role

			CreateMap<User, UserResponseDto>()
				.IncludeBase<User, UserDto>();

			// Organization mappings
			CreateMap<OrganizationCreateDto, Organization>()
				.ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.CreatedBy));

			CreateMap<Organization, OrganizationResponseDto>();

			CreateMap<Organization, OrganizationDto>();

			// Event mappings
			CreateMap<EventCreateDto, Event>()
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.Attendees, opt => opt.Ignore())
				.ForMember(dest => dest.AgendaItems, opt => opt.Ignore());

			CreateMap<EventUpdateDto, Event>()
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.Attendees, opt => opt.Ignore())
				.ForMember(dest => dest.AgendaItems, opt => opt.Ignore());

			CreateMap<Event, EventBasicDto>()
				.ForMember(dest => dest.AttendeeCount, opt => opt.MapFrom(src => src.Attendees.Count))
				.ForMember(dest => dest.AgendaItemCount, opt => opt.MapFrom(src => src.AgendaItems.Count));

			CreateMap<Event, EventBasicDetailedDto>();

			// Related mappings
			CreateMap<EventAttendee, EventAttendeeDto>()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
				.ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture));


			// Flow mappings
			CreateMap<FlowCreateDto, Flow>()
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.Triggers, opt => opt.Ignore())
				.ForMember(dest => dest.Actions, opt => opt.Ignore())
				.ForMember(dest => dest.Creator, opt => opt.Ignore())
				.ForMember(dest => dest.Updater, opt => opt.Ignore());

			CreateMap<FlowUpdateDto, Flow>()
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.IsActive, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.Triggers, opt => opt.Ignore())
				.ForMember(dest => dest.Actions, opt => opt.Ignore())
				.ForMember(dest => dest.Creator, opt => opt.Ignore())
				.ForMember(dest => dest.Updater, opt => opt.Ignore());

			// Response mappings
			CreateMap<Flow, FlowBasicDto>();

			CreateMap<Flow, FlowResponseDto>()
				.ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator))
				.ForMember(dest => dest.Updater, opt => opt.MapFrom(src => src.Updater));

			CreateMap<Flow, FlowDetailedDto>()
				.IncludeBase<Flow, FlowResponseDto>()
				.ForMember(dest => dest.Triggers, opt => opt.MapFrom(src => src.Triggers))
				.ForMember(dest => dest.Actions, opt => opt.MapFrom(src => src.Actions));

			// Trigger mappings
			CreateMap<TriggerCreateDto, Trigger>()
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.Flow, opt => opt.Ignore());

			CreateMap<TriggerUpdateDto, Trigger>()
				.ForMember(dest => dest.Flow, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.FlowId, opt => opt.Ignore());

			CreateMap<Trigger, TriggerDto>();
			CreateMap<Trigger, TriggerDetailedDto>();

			// File mappings
			CreateMap<FileCreateDto, EventFile>()
				.ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.Uploader, opt => opt.Ignore());

			CreateMap<FileUpdateDto, EventFile>()
				.ForMember(dest => dest.Url, opt => opt.Ignore())
				.ForMember(dest => dest.Type, opt => opt.Ignore())
				.ForMember(dest => dest.UploadedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UploadedBy, opt => opt.Ignore())
				.ForMember(dest => dest.ContentType, opt => opt.Ignore())
				.ForMember(dest => dest.SizeInBytes, opt => opt.Ignore())
				.ForMember(dest => dest.Uploader, opt => opt.Ignore());

			CreateMap<EventFile, FileDto>();
			CreateMap<EventFile, FileDetailedDto>();

			// Add these to your existing DbMappingProfile
			CreateMap<AgendaEntryCreateDto, AgendaEntry>();
			CreateMap<AgendaEntryUpdateDto, AgendaEntry>()
				.ForMember(dest => dest.EventId, opt => opt.Ignore())
				.ForMember(dest => dest.Event, opt => opt.Ignore());

			CreateMap<AgendaEntry, AgendaEntryDto>();

			// Action mappings
			CreateMap<ActionCreateDto, Action>()
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.Flow, opt => opt.Ignore());

			CreateMap<ActionUpdateDto, Action>()
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.FlowId, opt => opt.Ignore())
				.ForMember(dest => dest.Flow, opt => opt.Ignore());

			CreateMap<Action, ActionDto>();
			CreateMap<Action, ActionDetailedDto>();

		}
	}
}
