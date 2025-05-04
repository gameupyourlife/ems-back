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
				var expectedEvent = new EventInfoDTO
				{
					Id = eventId,
					Title = "Test Event",
					Status = 3
				};

				_eventRepoMock.Setup(x => x.GetByIdAsync(eventId))
					.ReturnsAsync(expectedEvent);

				// Act
				var result = await _eventService.GetEventByIdAsync(eventId);

				// Assert
				result.Should().NotBeNull();
				result.Id.Should().Be(eventId);
				result.Title.Should().Be(expectedEvent.Title);
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
					Category = 3
				};

				var expectedEvent = new EventInfoDTO
				{
					Id = Guid.NewGuid(),
					Title = eventDto.Title,
					Category = eventDto.Category
				};

				_eventRepoMock.Setup(x => x.AddAsync(eventDto))
					.ReturnsAsync(expectedEvent);

				// Act
				var result = await _eventService.CreateEventAsync(eventDto);

				// Assert
				result.Should().NotBeNull();
				result.Id.Should().NotBeEmpty();
				result.Title.Should().Be(eventDto.Title);
				result.Category.Should().Be(eventDto.Category);
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
				var statusDto = new EventInfoDTO { Status = 3 };

				var expectedEvent = new EventInfoDTO
				{
					Id = eventId,
					Status = statusDto.Status
				};

				_eventRepoMock.Setup(x => x.UpdateStatusAsync(eventId, statusDto))
					.ReturnsAsync(expectedEvent);

				// Act
				var result = await _eventService.UpdateEventStatusAsync(eventId, statusDto);

				// Assert
				result.Should().NotBeNull();
				result.Id.Should().Be(eventId);
				result.Status.Should().Be(statusDto.Status);
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