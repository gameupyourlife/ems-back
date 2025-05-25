using ems_back.Emails;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Exceptions;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Services;
using ems_back.Tests.Utilities;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using Xunit.Abstractions;

namespace ems_back.Tests.Services
{
    public class MailServiceTests : IDisposable
    {
        private readonly TestReportGenerator _report;
        private readonly Mock<IMailRepository> _mailRepositoryMock = new();
        private readonly Mock<IEventService> _eventServiceMock = new();
        private readonly Mock<ILogger<MailService>> _loggerMock = new();
        private readonly Mock<IUserService> _userServiceMock = new();
        private readonly Mock<IOrganizationRepository> _organizationRepoMock = new();
        private readonly MailService _mailService;



        public MailServiceTests(ITestOutputHelper output)
        {
            SmtpSettings smtpSettings = new SmtpSettings
            {
                Host = "smtp.example.com",
                Port = 587,
                Username = "donotreply@example.com",
                Password = "test",
            };
            var smtpOptions = Options.Create(smtpSettings);
            _report = new TestReportGenerator(output);

            _mailService = new MailService(
                _mailRepositoryMock.Object,
                smtpOptions,
                _eventServiceMock.Object,
                _loggerMock.Object,
                _userServiceMock.Object,
                _organizationRepoMock.Object
            );
        }

        #region CreateMailTests

        [Fact]
        public async Task CreateMailAsync_UserNotInOrg_ThrowsNotFoundException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var createMailDto = new CreateMailDto
            {
                Name = "Test Mail",
                Subject = "Test Subject",
                Body = "This is a test email body.",
                sendToAllParticipants = false
            };

            _userServiceMock.Setup(x => x.IsUserInOrgOrAdmin(orgId, userId))
                            .ReturnsAsync(false);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _mailService.CreateMailAsync(orgId, eventId, createMailDto, userId));
        }

        [Fact]
        public async Task CreateMailAsync_EventNotFound_ThrowsNotFoundException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var createMailDto = new CreateMailDto
            {
                Name = "Test Mail",
                Subject = "Test Subject",
                Body = "This is a test email body.",
                sendToAllParticipants = false
            };

            _userServiceMock.Setup(x => x.IsUserInOrgOrAdmin(orgId, userId))
                            .ReturnsAsync(true);

            _eventServiceMock.Setup(x => x.GetEventAsync(orgId, eventId, userId))
                            .ReturnsAsync((EventInfoDto)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _mailService.CreateMailAsync(orgId, eventId, createMailDto, userId));
        }

        [Fact]
        public async Task CreateMailAsync_Success_ReturnsMailDto()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var mailId = Guid.NewGuid();
            var recipient = Guid.NewGuid();
            var createMailDto = new CreateMailDto
            {
                Name = "Test Mail",
                Subject = "Test Subject",
                Body = "Test Body",
                Recipients = [recipient],
                sendToAllParticipants = false,
                ScheduledFor = DateTime.UtcNow.AddDays(1),
                Description = "Test Description"
            };
            var eventInfo = new EventInfoDto
            {
                Id = Guid.NewGuid(),
                Title = "Sample Event",
                OrganizationId = orgId,
                Location = "Sample Location",
                Description = "Sample Description",
                Category = "Sample Category",
                AttendeeCount = 10,
                Capacity = 100,
                Image = null,
                Status = EventStatus.ONGOING,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(2),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                CreatorName = "Sample Creator",
                UpdatedBy = userId,
                isAttending = true
            };

            _userServiceMock.Setup(x => x.IsUserInOrgOrAdmin(orgId, userId))
                            .ReturnsAsync(true);

            _eventServiceMock.Setup(x => x.GetEventAsync(orgId, eventId, userId))
                            .ReturnsAsync(eventInfo);

            _mailRepositoryMock.Setup(x => x.CreateMailAsync(eventId, It.IsAny<MailDto>()))
                            .ReturnsAsync(mailId);

            var result = await _mailService.CreateMailAsync(orgId, eventId, createMailDto, userId);

            Assert.NotNull(result);
            Assert.Equal(mailId, result.Id);
            Assert.Equal(createMailDto.Name, result.Name);
            Assert.Equal(createMailDto.Subject, result.Subject);
            Assert.Equal(createMailDto.Body, result.Body);
            Assert.Equal(createMailDto.Recipients, result.Recipients);
            Assert.Equal(createMailDto.sendToAllParticipants, result.sendToAllParticipants);
            Assert.True(result.IsUserCreated);
            Assert.Equal(userId, result.CreatedBy);
            Assert.Equal(userId, result.UpdatedBy);
            Assert.Equal(createMailDto.ScheduledFor, result.ScheduledFor);
            Assert.Equal(createMailDto.Description, result.Description);

            _mailRepositoryMock.Verify(x => x.CreateMailAsync(eventId, It.IsAny<MailDto>()), Times.Once);
        }

        #endregion

        #region SendMailTests

        [Fact]
        public async Task SendMailAsync_UserNotInOrg_ThrowsNotFoundException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var mailId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _userServiceMock.Setup(x => x.IsUserInOrgOrAdmin(orgId, userId))
                           .ReturnsAsync(false);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _mailService.SendMailAsync(orgId, eventId, mailId, userId));
        }

        [Fact]
        public async Task SendMailAsync_EventNotFound_ThrowsNotFoundException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var mailId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _userServiceMock.Setup(x => x.IsUserInOrgOrAdmin(orgId, userId))
                           .ReturnsAsync(true);

            _eventServiceMock.Setup(x => x.GetEventAsync(orgId, eventId, userId))
                           .ReturnsAsync((EventInfoDto)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _mailService.SendMailAsync(orgId, eventId, mailId, userId));
        }

        [Fact]
        public async Task SendMailAsync_MailNotFound_ThrowsNotFoundException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var mailId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventInfo = new EventInfoDto
            {
                Id = Guid.NewGuid(),
                Title = "Sample Event",
                OrganizationId = orgId,
                Location = "Sample Location",
                Category = "Sample Category",
                AttendeeCount = 10,
                Capacity = 100,
                Status = EventStatus.ONGOING,
                Start = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                CreatorName = "Sample Creator"
            };

            _userServiceMock.Setup(x => x.IsUserInOrgOrAdmin(orgId, userId))
                           .ReturnsAsync(true);

            _eventServiceMock.Setup(x => x.GetEventAsync(orgId, eventId, userId))
                           .ReturnsAsync(eventInfo);

            _mailRepositoryMock.Setup(x => x.GetMailByIdAsync(orgId, eventId, mailId))
                           .ReturnsAsync((MailDto)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _mailService.SendMailAsync(orgId, eventId, mailId, userId));
        }

        [Fact]
        public async Task SendMailAsync_NoRecipients_ThrowsNotFoundException()
        {
            var orgId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var mailId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventInfo = new EventInfoDto
            {
                Id = eventId,
                Title = "Sample Event",
                OrganizationId = orgId,
                Location = "Sample Location",
                Category = "Sample Category",
                AttendeeCount = 10,
                Capacity = 100,
                Status = EventStatus.ONGOING,
                Start = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                CreatorName = "Sample Creator"
            };

            var mail = new MailDto
            {
                Id = mailId,
                Name = "Test Mail",
                Subject = "Test Subject",
                Body = "Test Body",
                IsUserCreated = true,
                sendToAllParticipants = false,
                Recipients = new List<Guid>()
            };

            _userServiceMock.Setup(x => x.IsUserInOrgOrAdmin(orgId, userId))
                           .ReturnsAsync(true);

            _eventServiceMock.Setup(x => x.GetEventAsync(orgId, eventId, userId))
                           .ReturnsAsync(eventInfo);

            _mailRepositoryMock.Setup(x => x.GetMailByIdAsync(orgId, eventId, mailId))
                           .ReturnsAsync(mail);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _mailService.SendMailAsync(orgId, eventId, mailId, userId));
        }

        #endregion

        public void Dispose() => _report.Dispose();
    }
}
