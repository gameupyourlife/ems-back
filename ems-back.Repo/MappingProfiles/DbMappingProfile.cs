using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Agenda;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.File;
using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Flow.FlowsRun;
using ems_back.Repo.DTOs.Flow.FlowTemplate;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
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
					opt => opt.MapFrom(src => UserRole.User)); // Default role

			CreateMap<User, UserResponseDto>()
				.IncludeBase<User, UserDto>();

			// Organization mappings

			CreateMap<OrganizationCreateDto, Organization>()
				.ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.CreatedBy));

			CreateMap<Organization, OrganizationResponseDto>();

			CreateMap<Organization, OrganizationOverviewDto>();

			// Event mappings

			CreateMap<EventCreateDto, Event>()
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.Attendees, opt => opt.Ignore());

			CreateMap<EventInfoDto, Event>()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
				.ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start))
				.ForMember(dest => dest.End, opt => opt.MapFrom(src => src.End))
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
				.ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
				.ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
				.ForMember(dest => dest.AttendeeCount, opt => opt.MapFrom(src => src.AttendeeCount))
				.ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Capacity))
				.ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId))
				.ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

            CreateMap<EventUpdateDto, Event>();

            CreateMap<Event, EventOverviewDto>()
				.ForMember(dest => dest.Attendees, opt => opt.MapFrom(src => src.Attendees.Count));

            // Agenda mappings

            CreateMap<AgendaEntryDto, AgendaEntry>()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start))
				.ForMember(dest => dest.End, opt => opt.MapFrom(src => src.End))
				.ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId));

            // Related mappings
            CreateMap<EventAttendee, EventAttendeeDto>()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
				.ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture));


			// Flow mappings
			CreateMap<FlowCreateDto, Flow>()
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.Creator, opt => opt.Ignore())
				.ForMember(dest => dest.Updater, opt => opt.Ignore())
				.ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId))
				.ForMember(dest => dest.stillPending, opt => opt.MapFrom(src => src.stillPending))
				.ForMember(dest => dest.multipleRuns, opt => opt.MapFrom(src => src.multipleRuns))
				;


			CreateMap<FlowUpdateDto, Flow>()
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.IsActive, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.Creator, opt => opt.Ignore())
				.ForMember(dest => dest.Updater, opt => opt.Ignore())

				.ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId))
				.ForMember(dest => dest.stillPending, opt => opt.MapFrom(src => src.stillPending))
				.ForMember(dest => dest.multipleRuns, opt => opt.MapFrom(src => src.multipleRuns))
				;

			// Response mappings
			CreateMap<Flow, FlowDto>();

			CreateMap<Flow, FlowResponseDto>()
				.ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator))
				.ForMember(dest => dest.Updater, opt => opt.MapFrom(src => src.Updater));

			CreateMap<Flow, FlowDetailedDto>()
				.IncludeBase<Flow, FlowResponseDto>();

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
				.ForMember(dest => dest.SizeInBytes, opt => opt.Ignore())
				.ForMember(dest => dest.Uploader, opt => opt.Ignore());

			CreateMap<EventFile, FileDto>();

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



			CreateMap<FlowsRun, FlowsRunResponseDto>();
			CreateMap<FlowsRunCreateDto, FlowsRun>()
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => FlowRunStatus.Pending))
				.ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => DateTime.UtcNow));


			CreateMap<FlowTemplateCreateDto, FlowTemplate>()
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

			CreateMap<FlowTemplate, FlowTemplateResponseDto>();


			// Add these mappings to your existing DbMappingProfile class
			//CreateMap<OrganizationUser, OrganizationUserDto>()
			//	.ForMember(dest => dest.UserFullName,
			//		opt => opt.MapFrom(src => $"{src.UserFirstName} {src.UserLastName}"));

			//CreateMap<CreateOrganizationUserDto, OrganizationUser>()
			//	.ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
			//	.ForMember(dest => dest.IsOrganizationAdmin,
			//		opt => opt.MapFrom(src => src.UserRole == UserRole.Admin))
			//	.ForMember(dest => dest.UserFirstName, opt => opt.Ignore())  // Will be set from User
			//	.ForMember(dest => dest.UserLastName, opt => opt.Ignore())   // Will be set from User
			//	.ForMember(dest => dest.UserEmail, opt => opt.Ignore())      // Will be set from User
			//	.ForMember(dest => dest.OrganizationName, opt => opt.Ignore()) // Will be set from Organization
			//	.ForMember(dest => dest.OrganizationAddress, opt => opt.Ignore())
			//	.ForMember(dest => dest.OrganizationDescription, opt => opt.Ignore())
			//	.ForMember(dest => dest.OrganizationProfilePicture, opt => opt.Ignore())
			//	.ForMember(dest => dest.OrganizationWebsite, opt => opt.Ignore());

			//CreateMap<UpdateOrganizationUserDto, OrganizationUser>()
			//	.ForMember(dest => dest.UserId, opt => opt.Ignore())
			//	.ForMember(dest => dest.OrganizationId, opt => opt.Ignore())
			//	.ForMember(dest => dest.JoinedAt, opt => opt.Ignore())
			//	.ForMember(dest => dest.UserFirstName, opt => opt.Ignore())
			//	.ForMember(dest => dest.UserLastName, opt => opt.Ignore())
			//	.ForMember(dest => dest.UserEmail, opt => opt.Ignore())
			//	.ForMember(dest => dest.OrganizationName, opt => opt.Ignore())
			//	.ForMember(dest => dest.OrganizationAddress, opt => opt.Ignore())
			//	.ForMember(dest => dest.OrganizationDescription, opt => opt.Ignore())
			//	.ForMember(dest => dest.OrganizationProfilePicture, opt => opt.Ignore())
			//	.ForMember(dest => dest.OrganizationWebsite, opt => opt.Ignore());

		}
	}
}
