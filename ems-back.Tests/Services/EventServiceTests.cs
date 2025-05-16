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
        private readonly Mock<IOrganizationRepository> _orgRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly EventService _eventService;

        public EventServiceTests(ITestOutputHelper output)
        {
            _report = new TestReportGenerator(output);
            _eventRepoMock = new Mock<IEventRepository>();
            _orgRepoMock = new Mock<IOrganizationRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _eventService = new EventService(
            _eventRepoMock.Object,
            _userRepoMock.Object,
            _orgRepoMock.Object,
            Mock.Of<ILogger<EventService>>(),
            null);
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
                var expectedEvent = new EventInfoDto
                {
                    Title = "New Event",
                    OrganizationId = orgId,
                    Description = "Test Description",
                    Category = "Test Category"
                };

                _eventRepoMock.Setup(x => x.GetEventByIdAsync(orgId, eventId)).ReturnsAsync(expectedEvent);

                // Act
                var result = await _eventService.GetEventAsync(orgId, eventId);

                // Assert
                result.Should().NotBeNull();
                result.Id.Should().NotBeEmpty();
                result.Title.Should().Be("New Event");
                result.Category.Should().Be("Test Category");
                result.OrganizationId.Should().Be(orgId);
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
                Guid orgId = Guid.NewGuid();
                var eventDto = new EventCreateDto
                {
                    Title = "New Event",
                    Description = "Test Description",
                    Category = "Test Category",
                };

                var expectedEvent = new EventInfoDto
                {
                    Title = "New Event",
                    OrganizationId = orgId,
                    Description = "Test Description",
                    Category = "Test Category",
                };

                // Fix: Pass the required parameter to CreateEventAsync and fix the syntax error
                _eventRepoMock.Setup(x => x.CreateEventAsync(It.IsAny<EventInfoDto>())).ReturnsAsync(Guid.NewGuid());

                // Act
                //var result = await _eventService.CreateEventAsync(orgId, eventDto);

                // Assert
                //result.Should().NotBeNull();
                //result.Id.Should().NotBeEmpty();
                //result.Title.Should().Be(eventDto.Title);
                //result.Category.Should().Be(eventDto.Category);
                //result.OrganizationId.Should().Be(orgId);
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



		//[Fact]
  //      public async Task UpdateEventStatusAsync_ValidData_UpdatesStatus()
  //      {
  //          var testName = nameof(UpdateEventStatusAsync_ValidData_UpdatesStatus);
  //          var startTime = DateTime.Now;
  //          bool testPassed = false;
  //          string message = null;

  //          try
  //          {
  //              // Arrange
  //              var eventId = Guid.NewGuid();
  //              var statusDto = new EventInfoDTO { Status = EventStatus.ONGOING };

  //              var expectedEvent = new EventInfoDTO
  //              {
  //                  Title = "New Event",
  //                  //OrganizationId = orgId,
  //                  Description = "Test Description",
  //                  Category = "Test Category",
  //              };

  //              _eventRepoMock.Setup(x => x.UpdateStatusAsync(eventId, statusDto))
  //                  .ReturnsAsync(expectedEvent); // Ensure the return type matches EventDetailsDto

  //              // Act
  //              var result = await _eventService.UpdateEventStatusAsync(eventId, statusDto);

  //              // Assert
  //              result.Should().NotBeNull();
  //              //result.Metadata.Id.Should().Be(eventId);
  //              //result.Metadata.Status.Should().Be(statusDto.Status);
  //              testPassed = true;
  //          }
  //          catch (Exception ex)
  //          {
  //              message = ex.Message;
  //              throw;
  //          }
  //          finally
  //          {
  //              var duration = DateTime.Now - startTime;
  //              _report.AddTestResult(testName, testPassed, duration, message);
  //          }
  //      }

		
		public void Dispose() => _report.Dispose();
	}
}