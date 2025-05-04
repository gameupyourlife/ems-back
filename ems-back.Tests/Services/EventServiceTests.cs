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

namespace ems_back.Tests.Services
{
	public class EventServiceTests : IDisposable
	{
		private readonly TestReportGenerator _report;
		private readonly Mock<IEventRepository> _eventRepoMock;
		private readonly EventService _eventService;

        public EventServiceTests(ITestOutputHelper output)
        {
            _report = new TestReportGenerator(output);
            _eventRepoMock = new Mock<IEventRepository>();
            _eventService = new EventService(
            _eventRepoMock.Object,
            Mock.Of<IUserRepository>(), // Added missing IUserRepository mock
            Mock.Of<ILogger<EventService>>());
        }

		[Fact]
        public async Task GetByIdAsync_ExistingEvent_ReturnsEvent()
        {
            var testName = nameof(GetByIdAsync_ExistingEvent_ReturnsEvent);
            var startTime = DateTime.Now;
            bool testPassed = false;
            string message = null;

            try
            {
                // Arrange
                var eventId = Guid.NewGuid();
                var orgId = Guid.NewGuid();
                var expectedEvent = new EventDetailsDto // Updated to match the correct type
                {
                    Metadata = new EventInfoDTO(),
                    
                };

                _eventRepoMock.Setup(x => x.GetByIdAsync(orgId, eventId))
                    .ReturnsAsync(expectedEvent); // Ensure the return type matches EventDetailsDto

                // Act
                var result = await _eventService.GetEventByIdAsync(orgId, eventId);

                // Assert
                result.Should().NotBeNull();
                result.Metadata.Should().Be(eventId);
                testPassed = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                throw;
            }
            finally
            {
                var duration = DateTime.Now - startTime;
                _report.AddTestResult(testName, testPassed, duration, message);
            }
        }

		[Fact]
        public async Task CreateEventAsync_ValidData_ReturnsEvent()
        {
            var testName = nameof(CreateEventAsync_ValidData_ReturnsEvent);
            var startTime = DateTime.Now;
            bool testPassed = false;
            string message = null;

            try
            {
                // Arrange
                var eventDto = new EventCreateDto
                {
                    Title = "New Event",
                    Description = "Test Description",
                    Category = EventCategory.Networking,
                };

                var expectedEvent = new EventDetailsDto // Updated to match the correct return type
                {
                    Metadata = new EventInfoDTO(),
                };

                _eventRepoMock.Setup(x => x.AddAsync(eventDto))
                    .ReturnsAsync(expectedEvent); // Ensure the return type matches EventDetailsDto

                // Act
                var result = await _eventService.CreateEventAsync(eventDto);

                // Assert
                result.Should().NotBeNull();
                result.Metadata.Should().NotBeNull();
                testPassed = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                throw;
            }
            finally
            {
                var duration = DateTime.Now - startTime;
                _report.AddTestResult(testName, testPassed, duration, message);
            }
        }



		[Fact]
        public async Task UpdateEventStatusAsync_ValidData_UpdatesStatus()
        {
            var testName = nameof(UpdateEventStatusAsync_ValidData_UpdatesStatus);
            var startTime = DateTime.Now;
            bool testPassed = false;
            string message = null;

            try
            {
                // Arrange
                var eventId = Guid.NewGuid();
                var statusDto = new EventInfoDTO { Status = EventStatus.Draft };

                var expectedEvent = new EventDetailsDto // Ensure the type matches the repository method's return type
                {
                    Metadata = new EventInfoDTO
                    {
                        Id = eventId,
                        Status = statusDto.Status
                    }
                };

                _eventRepoMock.Setup(x => x.UpdateStatusAsync(eventId, statusDto))
                    .ReturnsAsync(expectedEvent); // Ensure the return type matches EventDetailsDto

                // Act
                var result = await _eventService.UpdateEventStatusAsync(eventId, statusDto);

                // Assert
                result.Should().NotBeNull();
                result.Metadata.Id.Should().Be(eventId);
                result.Metadata.Status.Should().Be(statusDto.Status);
                testPassed = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                throw;
            }
            finally
            {
                var duration = DateTime.Now - startTime;
                _report.AddTestResult(testName, testPassed, duration, message);
            }
        }

		
		public void Dispose() => _report.Dispose();
	}
}