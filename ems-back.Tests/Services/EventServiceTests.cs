using ems_back.Services;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;
using Moq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using ems_back.Repo.Interfaces;
using Microsoft.Extensions.Logging;
using ems_back.Tests.Utilities;
using ems_back.Repo.Services;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Exceptions;
using ems_back.Repo.DTOs.User;

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

        [Fact]
        public async Task CreateEventAsync_UserNotInOrg_ThrowMismatchException()
        {
            // Arrange
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
                Status = EventStatus.ONGOING,
                Start = DateTime.UtcNow.AddDays(1),
                End = DateTime.UtcNow.AddDays(2)
            };
            
            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(false);

            // Act & Assert
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
                Status = EventStatus.ONGOING,
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
                Status = EventStatus.ONGOING,
                Start = DateTime.UtcNow.AddDays(-1),
                End = DateTime.UtcNow.AddDays(1)
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
                Status = EventStatus.ONGOING,
                Start = DateTime.UtcNow.AddDays(1),
                End = DateTime.UtcNow.AddDays(2)
            };

            _userServiceMock.Setup(s => s.IsUserInOrgOrAdmin(orgId, userId)).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _eventService.CreateEventAsync(orgId, eventDto, userId));
        }

        [Fact]
        public async Task CreateEventAsync_EventAlreadyExists_ThrowsAlreadyExistsException()
        {
            // Arrange
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
                Status = EventStatus.ONGOING,
                Start = DateTime.UtcNow.AddDays(1), // Startdatum in der Zukunft!
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
                Status = EventStatus.ONGOING,
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
	
		public void Dispose() => _report.Dispose();
	}
}