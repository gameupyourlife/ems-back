using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;
using Moq;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using ems_back.Tests.Utilities;
using ems_back.Repo.Services;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Exceptions;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Models;
using ems_back.Repo.DTOs.Agenda;
using Quartz.Logging;
using Microsoft.AspNetCore.Identity;

namespace ems_back.Tests.Services
{
	public class EventServiceTests : IDisposable
	{
        private readonly TestReportGenerator _report;
        private readonly Mock<IEventRepository> _eventRepoMock = new();
        private readonly Mock<IOrganizationRepository> _orgRepoMock = new();
        private readonly Mock<IUserService> _userServiceMock = new();
        private readonly Mock<ILogger<EventService>> _loggerMock = new();
        private readonly EventService _eventService;
        private readonly Mock<IOrganizationService> _orgServiceMock = new();

        public EventServiceTests(ITestOutputHelper output)
        {
            _report = new TestReportGenerator(output);

            _eventService = new EventService(
                _eventRepoMock.Object,
                _userServiceMock.Object,
                _orgRepoMock.Object,
                _loggerMock.Object
        );
        }

        #region CreateEventTests

        [Fact]
        public async Task CreateEventAsync_UserNotInOrg_ThrowMismatchException()
        {
            var orgId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventDto = new EventCreateDto
            {
                Title = "New Event",
                Description = "Test Description",
                Category = "Test Category",
                Location = "Test Location",
                Capacity = 100,
                Image = "test-image.png",
                Status = EventStatus.Ongoing,
                Start = DateTime.UtcNow.AddDays(1),
                End = DateTime.UtcNow.AddDays(2)
            };
            
            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<MismatchException>(() => _eventService.CreateEventAsync(orgId, eventDto, userId));
        }

        [Fact]
        public async Task CreateEventAsync_StartDateInPast_ThrowsInvalidOperationException()
        {
            var orgId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventDto = new EventCreateDto 
            { 
                Title = "New Event",
                Description = "Test Description",
                Category = "Test Category",
                Location = "Test Location",
                Capacity = 100,
                Image = "test-image.png",
                Status = EventStatus.Ongoing,
                Start = DateTime.UtcNow.AddDays(-1),
                End = DateTime.UtcNow.AddDays(1)
            };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _eventService.CreateEventAsync(orgId, eventDto, userId));
        }

        [Fact]
        public async Task CreateEventAsync_StartAfterEnd_ThrowsInvalidOperationException()
        {
            var orgId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventDto = new EventCreateDto
            {
                Title = "New Event",
                Description = "Test Description",
                Category = "Test Category",
                Location = "Test Location",
                Capacity = 100,
                Image = "test-image.png",
                Status = EventStatus.Ongoing,
                Start = DateTime.UtcNow.AddHours(-1),
                End = DateTime.UtcNow.AddHours(1)
            };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _eventService.CreateEventAsync(orgId, eventDto, userId));
        }

        [Fact]
        public async Task CreateEventAsync_InvalidCapacity_ThrowsInvalidOperationException()
        {
            var orgId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventDto = new EventCreateDto
            {
                Title = "New Event",
                Description = "Test Description",
                Category = "Test Category",
                Location = "Test Location",
                Capacity = 0,
                Image = "test-image.png",
                Status = EventStatus.Ongoing,
                Start = DateTime.UtcNow.AddHours(1),
                End = DateTime.UtcNow.AddHours(2)
            };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _eventService.CreateEventAsync(orgId, eventDto, userId));
        }

        [Fact]
        public async Task CreateEventAsync_EventAlreadyExists_ThrowsAlreadyExistsException()
        {
            var orgId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventDto = new EventCreateDto
            {
                Title = "New Event",
                Description = "Test Description",
                Category = "Test Category",
                Location = "Test Location",
                Capacity = 100,
                Image = "test-image.png",
                Status = EventStatus.Ongoing,
                Start = DateTime.UtcNow.AddDays(1),
                End = DateTime.UtcNow.AddDays(2)
            };

            _userServiceMock
                .Setup(s => s.IsUserInOrgOrAdmin(orgId, userId))
                .ReturnsAsync(true);

            _userServiceMock
                .Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(new UserResponseDto
                {
                    Id = userId,
                    Email = "mail@test.com",
                    FirstName = "Test",
                    LastName = "User",
                });

            _eventRepoMock
                .Setup(r => r.GetEventAttendeeByIdAsync(orgId, userId))
                .ReturnsAsync((EventAttendeeDto?)null!);

            _eventRepoMock
                .Setup(r => r.GetEventByTitleAndDateAsync(eventDto.Title, eventDto.Start, orgId))
                .ReturnsAsync(new EventOverviewDto
                {
                    Id = Guid.NewGuid(),
                    Title = eventDto.Title,
                    Start = eventDto.Start,
                    Location = eventDto.Location,
                    Category = eventDto.Category,
                    AttendeeCount = 0,
                    Capacity = eventDto.Capacity,
                    Status = eventDto.Status
                });

            await Assert.ThrowsAsync<AlreadyExistsException>(() => _eventService.CreateEventAsync(orgId, eventDto, userId));
        }

        [Fact]
        public async Task CreateEventAsync_ValidData_ReturnsEventInfoDto()
        {
            var orgId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventDto = new EventCreateDto
            {
                Title = "New Event",
                Description = "Test Description",
                Category = "Test Category",
                Location = "Test Location",
                Capacity = 100,
                Image = "test-image.png",
                Status = EventStatus.Ongoing,
                Start = DateTime.UtcNow.AddDays(1),
                End = DateTime.UtcNow.AddDays(2)
            };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            _userServiceMock
                .Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(new UserResponseDto
                {
                    Id = userId,
                    Email = "mail@test.com",
                    FirstName = "Test",
                    LastName = "User",
                });

            _eventRepoMock
                .Setup(r => r.GetEventAttendeeByIdAsync(orgId, userId))
                .ReturnsAsync((EventAttendeeDto?)null!);


            _eventRepoMock.Setup(r => r.GetEventByTitleAndDateAsync(eventDto.Title, eventDto.Start, orgId)).ReturnsAsync((EventOverviewDto?)null!);
            _eventRepoMock.Setup(r => r.CreateEventAsync(It.IsAny<EventInfoDto>())).ReturnsAsync(Guid.NewGuid());

            var result = await _eventService.CreateEventAsync(orgId, eventDto, userId);

            Assert.Equal(eventDto.Title, result.Title);
            Assert.Equal(orgId, result.OrganizationId);
        }

        #endregion

        #region UpdateEventTests

        [Fact]
        public async Task UpdateEventAsync_UserNotInOrg_ThrowsMismatchException()
        {
            var orgId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var eventDto = new EventUpdateDto 
            { 
                Title = "Test",
                Location = "Testlocation",
                Description = "Test",
                Category = "Test",
                Status = EventStatus.Delayed,
                Image = "test.png",
                Start = DateTime.UtcNow.AddHours(1), 
                End = DateTime.UtcNow.AddHours(2), 
                Capacity = 10 
            };

            _userServiceMock
                .Setup(u => u.IsUserInOrgOrAdmin(orgId, userId))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<MismatchException>(() =>
                _eventService.UpdateEventAsync(orgId, eventId, eventDto, userId));
        }

        [Fact]
        public async Task UpdateEventAsync_StartInPast_ThrowsInvalidOperationException()
        {
            var orgId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var eventDto = new EventUpdateDto 
            {
                Title = "Test",
                Location = "Testlocation",
                Description = "Test",
                Category = "Test",
                Status = EventStatus.Delayed,
                Image = "test.png",
                Start = DateTime.UtcNow.AddHours(-1),
                End = DateTime.UtcNow.AddHours(1),
                Capacity = 10
            };

            _userServiceMock
                .Setup(u => u.IsUserInOrgOrAdmin(orgId, userId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _eventService.UpdateEventAsync(orgId, eventId, eventDto, userId));
        }

        [Fact]
        public async Task UpdateEventAsync_StartAfterEnd_ThrowsInvalidOperationException()
        {
            var orgId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var start = DateTime.UtcNow.AddHours(3);
            var end = DateTime.UtcNow.AddHours(1);
            var eventDto = new EventUpdateDto
            {
                Title = "Test",
                Location = "Testlocation",
                Description = "Test",
                Category = "Test",
                Status = EventStatus.Delayed,
                Image = "test.png",
                Start = DateTime.UtcNow.AddHours(2),
                End = DateTime.UtcNow.AddHours(1),
                Capacity = 10
            };

            _userServiceMock
                .Setup(u => u.IsUserInOrgOrAdmin(orgId, userId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _eventService.UpdateEventAsync(orgId, eventId, eventDto, userId));
        }

        [Fact]
        public async Task UpdateEventAsync_CapacityZero_ThrowsInvalidOperationException()
        {
            var orgId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var eventDto = new EventUpdateDto
            {
                Title = "Test",
                Location = "Testlocation",
                Description = "Test",
                Category = "Test",
                Status = EventStatus.Delayed,
                Image = "test.png",
                Start = DateTime.UtcNow.AddHours(1),
                End = DateTime.UtcNow.AddHours(2),
                Capacity = 0
            };

            _userServiceMock
                .Setup(u => u.IsUserInOrgOrAdmin(orgId, userId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _eventService.UpdateEventAsync(orgId, eventId, eventDto, userId));
        }

        [Fact]
        public async Task UpdateEventAsync_ValidData_CallsRepositoryAndReturnsEventInfo()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventDto = new EventUpdateDto
            {
                Title = "Test",
                Location = "Testlocation",
                Description = "Test",
                Category = "Test",
                Status = EventStatus.Delayed,
                Image = "test.png",
                Start = DateTime.UtcNow.AddHours(1),
                End = DateTime.UtcNow.AddHours(2),
                Capacity = 10
            };

            var expectedResult = new EventInfoDto 
            { 
                Id = eventId,
                OrganizationId = orgId,
                Title = "Test",
                Location = "Testlocation",
                Description = "Test",
                Category = "Test",
                Status = EventStatus.Delayed,
                Image = "test.png",
                Start = DateTime.UtcNow.AddHours(-1),
                End = DateTime.UtcNow.AddHours(1),
                AttendeeCount = 3,
                CreatedAt = DateTime.UtcNow,
                CreatorName = "Test Creator",
                UpdatedAt = DateTime.UtcNow,
                Capacity = 10
            };

            _userServiceMock
                .Setup(u => u.IsUserInOrgOrAdmin(orgId, userId))
                .ReturnsAsync(true);

            _eventRepoMock
                .Setup(r => r.UpdateEventAsync(orgId, eventId, eventDto, userId))
                .ReturnsAsync(expectedResult);

            var result = await _eventService.UpdateEventAsync(orgId, eventId, eventDto, userId);

            Assert.Equal(expectedResult.Id, result.Id);
            Assert.Equal(expectedResult.Title, result.Title);
            _eventRepoMock.Verify(r => r.UpdateEventAsync(orgId, eventId, eventDto, userId), Times.Once);
        }

        #endregion

        #region AddAttendeeTests

        [Fact]
        public async Task AddAttendeeToEventAsync_UserNotInOrg_ThrowsMismatchException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var attendeeDto = new EventAttendeeCreateDto { UserId = Guid.NewGuid() };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<MismatchException>(() =>
                _eventService.AddAttendeeToEventAsync(orgId, eventId, attendeeDto, userId));
        }

        [Fact]
        public async Task AddAttendeeToEventAsync_UserAlreadyRegistered_ThrowsAlreadyExistsException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var attendeeId = Guid.NewGuid();
            var attendeeDto = new EventAttendeeCreateDto { UserId = attendeeId };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            _eventRepoMock.Setup(r => r.GetEventAttendeeByIdAsync(eventId, attendeeId))
                          .ReturnsAsync(new EventAttendeeDto
                          {
                              UserId = attendeeId,
                              UserEmail = "testuser@example.com",
                              UserName = "Test User",
                              Status = AttendeeStatus.Participant,
                              RegisteredAt = DateTime.UtcNow
                          });

            await Assert.ThrowsAsync<AlreadyExistsException>(() =>
                _eventService.AddAttendeeToEventAsync(orgId, eventId, attendeeDto, userId));
        }

        [Fact]
        public async Task AddAttendeeToEventAsync_EventNotFound_ThrowsNotFoundException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var attendeeDto = new EventAttendeeCreateDto { UserId = Guid.NewGuid() };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            _eventRepoMock.Setup(r => r.GetEventAttendeeByIdAsync(eventId, attendeeDto.UserId))
                          .ReturnsAsync((EventAttendeeDto?)null!);

            _eventRepoMock.Setup(r => r.GetEventByIdAsync(orgId, eventId))
                          .ReturnsAsync((EventInfoDto?)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _eventService.AddAttendeeToEventAsync(orgId, eventId, attendeeDto, userId));
        }

        [Fact]
        public async Task AddAttendeeToEventAsync_EventIsFull_ThrowsNoCapacityException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var attendeeDto = new EventAttendeeCreateDto { UserId = Guid.NewGuid() };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            _eventRepoMock.Setup(r => r.GetEventAttendeeByIdAsync(eventId, attendeeDto.UserId))
                          .ReturnsAsync((EventAttendeeDto?)null!);

            _eventRepoMock.Setup(r => r.GetEventByIdAsync(orgId, eventId))
                          .ReturnsAsync(new EventInfoDto
                          {
                              Title = "Sample Event",
                              OrganizationId = orgId,
                              Location = "Sample Location",
                              Category = "Sample Category",
                              Status = EventStatus.Ongoing,
                              Start = DateTime.UtcNow.AddHours(1),
                              CreatedAt = DateTime.UtcNow,
                              UpdatedAt = DateTime.UtcNow,
                              CreatorName = "Test Creator",
                              AttendeeCount = 100,
                              Capacity = 100
                          });

            await Assert.ThrowsAsync<NoCapacityException>(() =>
                _eventService.AddAttendeeToEventAsync(orgId, eventId, attendeeDto, userId));
        }

        [Fact]
        public async Task AddAttendeeToEventAsync_Valid_ReturnsAttendeeInfo()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var attendeeId = Guid.NewGuid();
            var attendeeDto = new EventAttendeeCreateDto { UserId = attendeeId };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            _eventRepoMock.Setup(r => r.GetEventAttendeeByIdAsync(eventId, attendeeId))
                          .ReturnsAsync((EventAttendeeDto?)null!);

            _eventRepoMock.Setup(r => r.GetEventByIdAsync(orgId, eventId))
                          .ReturnsAsync(new EventInfoDto
                          {
                              Title = "Sample Event",
                              OrganizationId = orgId,
                              Location = "Sample Location",
                              Category = "Sample Category",
                              Status = EventStatus.Ongoing,
                              Start = DateTime.UtcNow.AddHours(1),
                              CreatedAt = DateTime.UtcNow,
                              UpdatedAt = DateTime.UtcNow,
                              CreatorName = "Test Creator",
                              AttendeeCount = 10,
                              Capacity = 100
                          });

            _eventRepoMock.Setup(r => r.AddAttendeeToEventAsync(It.IsAny<EventAttendee>()))
                          .ReturnsAsync(true);

            _userServiceMock.Setup(s => s.GetUserByIdAsync(attendeeId))
                            .ReturnsAsync(new UserResponseDto 
                            {
                                FirstName = "John",
                                LastName = "Doe",
                                Email = "test@example.com",
                            });

            var result = await _eventService.AddAttendeeToEventAsync(orgId, eventId, attendeeDto, userId);

            Assert.Equal(attendeeId, result.UserId);
            Assert.Equal("test@example.com", result.UserEmail);
            Assert.Equal("John Doe", result.UserName);
        }

        #endregion

        #region AddEventOrganizerTests

        [Fact]
        public async Task AddEventOrganizerAsync_UserNotInOrg_ThrowsMismatchException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var organizerId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<MismatchException>(() =>
                _eventService.AddEventOrganizerAsync(orgId, eventId, organizerId, userId));
        }

        [Fact]
        public async Task AddEventOrganizerAsync_OrganizerNotInOrg_ThrowsMismatchException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var organizerId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, organizerId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<MismatchException>(() =>
                _eventService.AddEventOrganizerAsync(orgId, eventId, organizerId, userId));
        }

        [Fact]
        public async Task AddEventOrganizerAsync_EventNotFound_ThrowsNotFoundException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var organizerId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, organizerId)).ReturnsAsync(true);

            _eventRepoMock.Setup(r => r.GetEventByIdAsync(orgId, eventId)).ReturnsAsync((EventInfoDto?)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _eventService.AddEventOrganizerAsync(orgId, eventId, organizerId, userId));
        }

        [Fact]
        public async Task AddEventOrganizerAsync_OrganizerAlreadyExists_ThrowsAlreadyExistsException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var organizerId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, organizerId)).ReturnsAsync(true);

            _eventRepoMock.Setup(r => r.GetEventByIdAsync(orgId, eventId)).ReturnsAsync(new EventInfoDto
            {
                Id = eventId,
                Title = "Sample Event",
                OrganizationId = orgId,
                Location = "Sample Location",
                Description = "Sample Description",
                Category = "Sample Category",
                AttendeeCount = 0,
                Capacity = 100,
                Image = "sample-image.png",
                Status = EventStatus.Ongoing,
                Start = DateTime.UtcNow.AddHours(1),
                End = DateTime.UtcNow.AddHours(2),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                CreatorName = "Test Creator"
            });
                
            _eventRepoMock.Setup(r => r.GetEventOrganizerAsync(eventId, organizerId)).ReturnsAsync(new EventOrganizer
            {
                EventId = eventId,
                UserId = organizerId,
            });

            await Assert.ThrowsAsync<AlreadyExistsException>(() =>
                _eventService.AddEventOrganizerAsync(orgId, eventId, organizerId, userId));
        }

        [Fact]
        public async Task AddEventOrganizerAsync_Valid_ReturnsTrue()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var organizerId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, organizerId)).ReturnsAsync(true);

            _eventRepoMock.Setup(r => r.GetEventByIdAsync(orgId, eventId)).ReturnsAsync(new EventInfoDto
            {
                Id = eventId,
                Title = "Sample Event",
                OrganizationId = orgId,
                Location = "Sample Location",
                Description = "Sample Description",
                Category = "Sample Category", 
                AttendeeCount = 0, 
                Capacity = 100, 
                Image = "sample-image.png",
                Status = EventStatus.Ongoing, 
                Start = DateTime.UtcNow.AddHours(1), 
                End = DateTime.UtcNow.AddHours(2),
                CreatedAt = DateTime.UtcNow.AddDays(-1), 
                UpdatedAt = DateTime.UtcNow, 
                CreatorName = "Test Creator" 
            });

            _eventRepoMock.Setup(r => r.GetEventOrganizerAsync(eventId, organizerId)).ReturnsAsync((EventOrganizer?)null!);

            _eventRepoMock.Setup(r => r.AddEventOrganizerAsync(orgId, eventId, organizerId)).ReturnsAsync(true);

            _userServiceMock.Setup(s => s.UpdateUserRoleAsync(organizerId, It.IsAny<UserUpdateRoleDto>())).ReturnsAsync(true);

            var result = await _eventService.AddEventOrganizerAsync(orgId, eventId, organizerId, userId);

            Assert.True(result);
        }

        #endregion

        #region AddAgendaTests

        [Fact]
        public async Task AddAgendaEntryToEventAsync_UserNotInOrg_ThrowsMismatchException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var agendaEntryDto = new AgendaEntryCreateDto { Title = "Title", Description = "Desc", Start = DateTime.UtcNow, End = DateTime.UtcNow.AddHours(1) };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<MismatchException>(() =>
                _eventService.AddAgendaEntryToEventAsync(orgId, eventId, agendaEntryDto, userId));
        }

        [Fact]
        public async Task AddAgendaEntryToEventAsync_EventNotFound_ThrowsNotFoundException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var agendaEntryDto = new AgendaEntryCreateDto { Title = "Agenda", Description = "Description", Start = DateTime.UtcNow, End = DateTime.UtcNow.AddHours(1) };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);
            _eventRepoMock.Setup(r => r.GetEventByIdAsync(orgId, eventId)).ReturnsAsync((EventInfoDto?)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _eventService.AddAgendaEntryToEventAsync(orgId, eventId, agendaEntryDto, userId));
        }

        [Fact]
        public async Task AddAgendaEntryToEventAsync_ValidRequest_ReturnsAgendaEntryDto()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var agendaEntryId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var agendaEntryDto = new AgendaEntryCreateDto
            {
                Title = "Session 1",
                Description = "Intro",
                Start = now,
                End = now.AddHours(1)
            };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);
            _eventRepoMock.Setup(r => r.GetEventByIdAsync(orgId, eventId)).ReturnsAsync(new EventInfoDto
            {
                Id = eventId,
                Title = "Sample Event",
                OrganizationId = orgId,
                Location = "Sample Location",
                Description = "Sample Description",
                Category = "Sample Category",
                AttendeeCount = 0,
                Capacity = 100,
                Image = "sample-image.png",
                Status = EventStatus.Ongoing,
                Start = DateTime.UtcNow.AddHours(1),
                End = DateTime.UtcNow.AddHours(2),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                CreatorName = "Test Creator"
            });

            _eventRepoMock.Setup(r => r.AddAgendaEntryToEventAsync(It.IsAny<AgendaEntryDto>()))
                .ReturnsAsync(agendaEntryId);

            var result = await _eventService.AddAgendaEntryToEventAsync(orgId, eventId, agendaEntryDto, userId);

            Assert.NotNull(result);
            Assert.Equal(agendaEntryDto.Title, result.Title);
            Assert.Equal(agendaEntryDto.Description, result.Description);
            Assert.Equal(agendaEntryDto.Start, result.Start);
            Assert.Equal(agendaEntryDto.End, result.End);
            Assert.Equal(agendaEntryId, result.Id);
        }


        #endregion

        #region UpdateAgendaTests

        [Fact]
        public async Task UpdateAgendaEntryAsync_UserNotInOrg_ThrowsMismatchException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var agendaId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new AgendaEntryCreateDto
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Start = DateTime.UtcNow.AddHours(1),
                End = DateTime.UtcNow.AddHours(2)
            };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<MismatchException>(() =>
                _eventService.UpdateAgendaEntryAsync(orgId, eventId, agendaId, dto, userId));
        }

        [Fact]
        public async Task UpdateAgendaEntryAsync_EventNotFound_ThrowsNotFoundException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var agendaId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new AgendaEntryCreateDto
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Start = DateTime.UtcNow.AddHours(1),
                End = DateTime.UtcNow.AddHours(2)
            }; 

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            _eventRepoMock.Setup(r => r.GetEventByIdAsync(orgId, eventId)).ReturnsAsync((EventInfoDto?)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _eventService.UpdateAgendaEntryAsync(orgId, eventId, agendaId, dto, userId));
        }

        [Fact]
        public async Task UpdateAgendaEntryAsync_AgendaEntryNotFound_ThrowsNotFoundException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var agendaId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new AgendaEntryCreateDto
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Start = DateTime.UtcNow.AddHours(1),
                End = DateTime.UtcNow.AddHours(2)
            }; 

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);
            
            _eventRepoMock.Setup(r => r.GetEventByIdAsync(orgId, eventId)).ReturnsAsync(new EventInfoDto
            {
                Id = eventId,
                Title = "Sample Event",
                OrganizationId = orgId,
                Location = "Sample Location",
                Description = "Sample Description",
                Category = "Sample Category",
                AttendeeCount = 0,
                Capacity = 100,
                Image = "sample-image.png",
                Status = EventStatus.Ongoing,
                Start = DateTime.UtcNow.AddHours(1),
                End = DateTime.UtcNow.AddHours(2),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                CreatorName = "Test Creator"
            });

            _eventRepoMock.Setup(r => r.GetAgendaEntryByIdAsync(agendaId)).ReturnsAsync((AgendaEntry?)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _eventService.UpdateAgendaEntryAsync(orgId, eventId, agendaId, dto, userId));
        }

        [Fact]
        public async Task UpdateAgendaEntryAsync_ValidRequest_ReturnsUpdatedAgenda()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var agendaId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var start = DateTime.UtcNow;
            var end = start.AddHours(1);

            var dto = new AgendaEntryCreateDto
            {
                Title = "Updated Title",
                Description = "Updated Desc",
                Start = start,
                End = end
            };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            
            _eventRepoMock.Setup(r => r.GetEventByIdAsync(orgId, eventId)).ReturnsAsync(new EventInfoDto 
            {
                Id = eventId,
                Title = "Sample Event",
                OrganizationId = orgId,
                Location = "Sample Location",
                Description = "Sample Description",
                Category = "Sample Category",
                AttendeeCount = 0,
                Capacity = 100,
                Image = "sample-image.png",
                Status = EventStatus.Ongoing,
                Start = DateTime.UtcNow.AddHours(1),
                End = DateTime.UtcNow.AddHours(2),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                CreatorName = "Test Creator"
            });
            _eventRepoMock.Setup(r => r.GetAgendaEntryByIdAsync(agendaId)).ReturnsAsync(new AgendaEntry
            {
                Id = agendaId,
                Title = "Old Title",
                Description = "Old Description",
                Start = start.AddHours(-1),
                End = end.AddHours(-1),
                EventId = eventId
            });
            _eventRepoMock.Setup(r => r.UpdateAgendaEntryAsync(agendaId, eventId, It.IsAny<AgendaEntryDto>())).ReturnsAsync(true);

            var result = await _eventService.UpdateAgendaEntryAsync(orgId, eventId, agendaId, dto, userId);

            Assert.Equal(agendaId, result.Id);
            Assert.Equal(dto.Title, result.Title);
            Assert.Equal(dto.Description, result.Description);
            Assert.Equal(dto.Start, result.Start);
            Assert.Equal(dto.End, result.End);
            Assert.Equal(eventId, result.EventId);
        }

        #endregion

        public void Dispose() => _report.Dispose();
	}
}