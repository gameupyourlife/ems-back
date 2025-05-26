using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Exceptions;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ems_back.Emails;
using ems_back.Repo.Models;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Security;
using ems_back.Repo.Repository;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using ems_back.Repo.Models.Types;
using ems_back.Repo.DTOs.Mail;

namespace ems_back.Repo.Services
{
    public class MailService : Interfaces.Service.IMailService
    {
        private readonly IMailRepository _mailRepository;
        private readonly IEventService _eventService;
        private readonly ILogger<MailService> _logger;
        private readonly SmtpSettings _smtp;
        private readonly IUserService _userService;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IMailQueueService _mailQueueService;
        private readonly IEventRepository _eventRepository;

        public MailService(
            IMailRepository emailRepository, 
            IOptions<SmtpSettings> options,
            IEventService eventService,
            ILogger<MailService> logger,
            IUserService userService,
            IOrganizationRepository organizationRepository,
            IMailQueueService mailQueueService,
            IEventRepository eventRepository)
        {
            _mailRepository = emailRepository;
            _eventService = eventService;
            _logger = logger;
            _smtp = options.Value;
            _userService = userService;
            _organizationRepository = organizationRepository;
            _mailQueueService = mailQueueService;
            _eventRepository = eventRepository;
        }

        public async Task<bool> ExistsOrg(Guid orgId)
        {
            var org = await _organizationRepository.GetOrganizationByIdAsync(orgId);
            if (org == null)
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return false;
            }

            return true;
        }

        public async Task<MailDto> CreateMailAsync(
            Guid orgId, 
            Guid eventId, 
            CreateMailDto createMailDto,
            Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not in organization {OrgId}", userId, orgId);
                throw new NotFoundException("User not found in organization");
            }

            var eventInfo = await _eventService.GetEventAsync(orgId, eventId, userId);
            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} does not exist", eventId);
                throw new NotFoundException("Event not found");
            }

            var mail = new MailDto
            {
                Name = createMailDto.Name,
                Subject = createMailDto.Subject,
                Body = createMailDto.Body,
                Recipients = createMailDto.Recipients,
                sendToAllParticipants = createMailDto.sendToAllParticipants,
                IsUserCreated = true,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = userId,
                ScheduledFor = createMailDto.ScheduledFor,
                Description = createMailDto.Description
            };

            var mailId = await _mailRepository.CreateMailAsync(eventId, mail);
            if (mailId == Guid.Empty)
            {
                _logger.LogWarning("Mail creation failed for event {EventId}", eventId);
                throw new DbUpdateException("Mail creation failed");
            }

            mail.Id = mailId;

            return mail;
        }

        public async Task<bool> DeleteMailAsync(Guid orgId, Guid eventId, Guid mailId, Guid userId)
        {

            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not in organization {OrgId}", userId, orgId);
                throw new NotFoundException("User not found in organization");
            }

            var eventInfo = await _eventService.GetEventAsync(orgId, eventId, userId);
            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} does not exist", eventId);
                throw new NotFoundException("Event not found");
            }

            var mail = await _mailRepository.GetMailByIdAsync(orgId, eventId, mailId);
            if (mail == null)
            {
                _logger.LogWarning("Mail with id {MailId} does not exist", mailId);
                throw new NotFoundException("Mail not found");
            }
            var result = await _mailRepository.DeleteMailAsync(orgId, eventId, mailId);
            if (!result)
            {
                _logger.LogWarning("Mail deletion failed for event {EventId}", eventId);
                throw new DbUpdateException("Mail deletion failed");
            }

            return result;
        }

        public async Task<MailDto> GetMailByIdAsync(Guid orgId, Guid eventId, Guid mailId, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not in organization {OrgId}", mailId, orgId);
                throw new NotFoundException("User not found in organization");
            }

            var eventInfo = await _eventService.GetEventAsync(orgId, eventId, userId);
            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} does not exist", eventId);
                throw new NotFoundException("Event not found");
            }

            var mail = await _mailRepository.GetMailByIdAsync(orgId, eventId, mailId);
            if (mail == null)
            {
                _logger.LogWarning("Mail with id {MailId} does not exist", mailId);
                throw new NotFoundException("Mail not found");
            }

            return mail;
        }

        public async Task<IEnumerable<MailDto>> GetMailsForEventAsync(
            Guid orgId, 
            Guid eventId, 
            Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not in organization {OrgId}", userId, orgId);
                throw new NotFoundException("User not found in organization");
            }

            var mails = await _mailRepository.GetMailsForEventAsync(orgId, eventId);
            if (mails == null)
            {
                _logger.LogWarning("No mails found for event {EventId}", eventId);
                throw new NotFoundException("No Mails found");
            }

            return mails;
        }

        public async Task<MailDto> UpdateMailAsync(
            Guid orgId,
            Guid eventId,
            Guid mailId,
            CreateMailDto mailDto,
            Guid userId)
        {
            var eventInfo = await _eventService.GetEventAsync(orgId, eventId, userId);
            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} does not exist", eventId);
                throw new NotFoundException("Event not found");
            }
            if (eventInfo.OrganizationId != orgId)
            {
                _logger.LogWarning("Event is not in given org");
                throw new MismatchException("Given event is not in given Org");
            }

            if (await _mailRepository.GetMailByIdAsync(orgId, eventId, mailId) == null)
            {
                _logger.LogWarning("Mail with id {MailId} does not exist", mailId);
                throw new NotFoundException("Mail not found");
            }

            return await _mailRepository.UpdateMailAsync(orgId, eventId, mailId, mailDto, userId);
        }

        public async Task SendMailByIdAsync(Guid orgId, Guid eventId, Guid mailId, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not in organization {OrgId}", userId, orgId);
                throw new NotFoundException("User not found in organization");
            }

            var eventInfo = await _eventService.GetEventAsync(orgId, eventId, userId);
            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} does not exist", eventId);
                throw new NotFoundException("Event not found");
            }

            var mail = await GetMailByIdAsync(orgId, eventId, mailId, userId);
            if (mail == null)
            {
                _logger.LogWarning("Mail with id {MailId} does not exist", mailId);
                throw new NotFoundException("Mail not found");
            }

            var recipientIds = await GetRecipientIdsAsync(mail, orgId, eventId, userId);

            foreach (var recipientId in recipientIds)
            {
                var user = await _userService.GetUserByIdAsync(recipientId);
                await _mailQueueService.EnqueueAsnyc(user.Email, user.FullName, mail.Subject, mail.Body);
            }
        }

        private async Task<IEnumerable<Guid>> GetRecipientIdsAsync(MailDto mail, Guid orgId, Guid eventId, Guid userId)
        {
            if (mail.sendToAllParticipants)
            {
                var participants = await _eventService.GetAllEventAttendeesAsync(orgId, eventId, userId);
                if (participants == null || !participants.Any())
                {
                    _logger.LogWarning("No participants found for event with id {EventId}", eventId);
                    throw new NotFoundException("No participants found for the event");
                }
                return participants.Select(p => p.UserId);
            }

            if (mail.Recipients == null || !mail.Recipients.Any())
            {
                _logger.LogWarning("No recipients found for mail with id {MailId}", mail.Id);
                throw new NotFoundException("No recipients found for the mail");
            }

            return mail.Recipients;
        }

        public async Task SendMailWithDtoAsync(Guid orgId, Guid eventId, CreateMailDto mail, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not in organization {OrgId}", userId, orgId);
                throw new NotFoundException("User not found in organization");
            }

            var eventInfo = await _eventService.GetEventAsync(orgId, eventId, userId);
            if (eventInfo == null)
            {
                _logger.LogWarning("Event with id {EventId} does not exist", eventId);
                throw new NotFoundException("Event not found");
            }

            var mailDto = new MailDto
            {
                Name = mail.Name,
                Subject = mail.Subject,
                Body = mail.Body,
                Recipients = mail.Recipients,
                sendToAllParticipants = mail.sendToAllParticipants,
                IsUserCreated = true,
            };

            var recipientIds = await GetRecipientIdsAsync(mailDto, orgId, eventId, userId);

            foreach (var recipientId in recipientIds)
            {
                var user = await _userService.GetUserByIdAsync(recipientId);
                await _mailQueueService.EnqueueAsnyc(user.Email, user.FullName, mailDto.Subject, mailDto.Body);
            }
        }

        public async Task SendMailManualAsync(Guid orgId, MailManualDto mailDto, Guid userId)
        {
            if (!await _userService.IsUserInOrgOrAdmin(orgId, userId))
            {
                _logger.LogWarning("User with id {UserId} is not in organization {OrgId}", userId, orgId);
                throw new NotFoundException("User not found in organization");
            }

            foreach (var recipient in mailDto.recipentMails)
            {
                _logger.LogInformation("Mail sent to {Recipient}", recipient);
                await _mailQueueService.EnqueueAsnyc(recipient, "" , mailDto.Subject, mailDto.Body);
            }
        }

        public async Task SendMailByFlowAsync(Guid orgId, Guid eventId, Guid mailId)
        {
            var mail = await _mailRepository.GetMailByIdAsync(orgId, eventId, mailId);
            IEnumerable<Guid>? selectedParticipants;

            if (mail == null)
            {
                _logger.LogWarning("Mail with id {MailId} does not exist", mailId);
                throw new NotFoundException("Mail not found");
            }
            if (mail.sendToAllParticipants)
            {
                var participants = await _eventRepository.GetAllEventAttendeesAsync(orgId, eventId);
                if (participants == null || !participants.Any())
                {
                    _logger.LogWarning("No participants found for event with id {EventId}", eventId);
                    throw new NotFoundException("No participants found for the event");
                }
                selectedParticipants = participants.Select(p => p.UserId);
            }
            else
            {
                selectedParticipants = mail.Recipients;
            }

            if (selectedParticipants == null || !selectedParticipants.Any())
            {
                _logger.LogWarning("No recipients found for mail with id {MailId}", mail.Id);
                throw new NotFoundException("No recipients found for the mail");
            }

            foreach (var recipientId in selectedParticipants)
            {
                var user = await _userService.GetUserByIdAsync(recipientId);
                await _mailQueueService.EnqueueAsnyc(user.Email, user.FullName, mail.Subject, mail.Body);
            }
        }
    }
}
