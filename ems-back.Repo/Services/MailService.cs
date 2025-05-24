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

        public MailService(
            IMailRepository emailRepository, 
            IOptions<SmtpSettings> options,
            IEventService eventService,
            ILogger<MailService> logger,
            IUserService userService,
            IOrganizationRepository organizationRepository)
        {
            _mailRepository = emailRepository;
            _eventService = eventService;
            _logger = logger;
            _smtp = options.Value;
            _userService = userService;
            _organizationRepository = organizationRepository;
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
            if (!await ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                throw new NotFoundException("Organization not found");
            }

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
            if (eventInfo.OrganizationId != orgId) {
                _logger.LogWarning("Event is not in given org");
                throw new MismatchException("Given event is not in given Org");
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
            if (!await ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                throw new NotFoundException("Organization not found");
            }

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
            if (eventInfo.OrganizationId != orgId)
            {
                _logger.LogWarning("Event is not in given org");
                throw new MismatchException("Given event is not in given Org");
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
            if (!await ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                throw new NotFoundException("Organization not found");
            }

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
            if (eventInfo.OrganizationId != orgId)
            {
                _logger.LogWarning("Event is not in given org");
                throw new MismatchException("Given event is not in given Org");
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
            if (!await ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                throw new NotFoundException("Organization not found");
            }

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
            if (!await ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                throw new NotFoundException("Organization not found");
            }

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

        public async Task SendMailAsync(Guid orgId, Guid eventId, Guid mailId, Guid userId)
        {
            if (!await ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                throw new NotFoundException("Organization not found");
            }

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
            if (eventInfo.OrganizationId != orgId)
            {
                _logger.LogWarning("Event is not in given org");
                throw new MismatchException("Given event is not in given Org");
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
                var message = CreateMimeMessage(mail.Subject, mail.Body, user.Email, $"{user.FirstName} {user.LastName}");

                await SendEmailAsync(message);
            }
        }

        private async Task<IEnumerable<Guid>> GetRecipientIdsAsync(MailDto mail, Guid orgId, Guid eventId, Guid userId)
        {
            if (mail.sendToAllParticipants)
            {
                var participants = await _eventService.GetAllEventAttendeesAsync(orgId, eventId, userId);
                return participants.Select(p => p.UserId);
            }

            if (mail.Recipients == null || !mail.Recipients.Any())
            {
                _logger.LogWarning("No recipients found for mail with id {MailId}", mail.Id);
                throw new NotFoundException("No recipients found for the mail");
            }

            return mail.Recipients;
        }

        private MimeMessage CreateMimeMessage(string subject, string body, string recipientEmail, string recipientName)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("NextGenDevelopment", _smtp.Username));
            message.To.Add(new MailboxAddress(recipientName, recipientEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };
            return message;
        }

        private async Task SendEmailAsync(MimeMessage message)
        {
            using var smtp = new SmtpClient();
            smtp.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await smtp.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_smtp.Username, _smtp.Password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendMailWithDtoAsync(Guid orgId, Guid eventId, CreateMailDto mail, Guid userId)
        {
            if (!await ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                throw new NotFoundException("Organization not found");
            }

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
            if (eventInfo.OrganizationId != orgId)
            {
                _logger.LogWarning("Event is not in given org");
                throw new MismatchException("Given event is not in given Org");
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
                var message = CreateMimeMessage(
                    mail.Subject,
                    mail.Body,
                    user.Email,
                    $"{user.FirstName} {user.LastName}"
                );

                await SendEmailAsync(message);
            }
        }

        public async Task SendMailManualAsync(MailManualDto mailDto)
        {

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("NextGenDevelopment", _smtp.Username));
            email.Subject = mailDto.Subject;
            email.Body = new TextPart("plain") { Text = mailDto.Body};

            foreach (var recipient in mailDto.recipentMails)
            {
                email.To.Clear();
                email.To.Add(MailboxAddress.Parse(recipient));

                using (var smtp = new SmtpClient())
                {
                    smtp.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
                    smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await smtp.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(_smtp.Username, _smtp.Password);
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                }
            }
        }
    }
}
