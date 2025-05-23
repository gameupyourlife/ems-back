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

namespace ems_back.Repo.Services
{
    public class MailService : Interfaces.Service.IMailService
    {
        private readonly IMailRepository _mailRepository;
        private readonly IEventService _eventService;
        private readonly ILogger<MailService> _logger;
        private readonly SmtpSettings _smtp;
        private readonly IUserService _userService;

        public MailService(
            IMailRepository emailRepository, 
            IOptions<SmtpSettings> options, 
            IEventService eventService, 
            ILogger<MailService> logger,
            IUserService userService)
        {
            _mailRepository = emailRepository;
            _eventService = eventService;
            _logger = logger;
            _smtp = options.Value;
            _userService = userService;
        }

        public Task<MailDto> CreateMailAsync(Guid orgId, Guid eventId, CreateMailDto createMailDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteMailAsync(Guid orgId, Guid eventId, Guid mailId)
        {
            throw new NotImplementedException();
        }

        public Task<MailDto> GetMailByIdAsync(Guid orgId, Guid eventId, Guid mailId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MailDto>> GetMailsForEventAsync(Guid orgId, Guid eventId)
        {
            var mails = _mailRepository.GetMailsForEventAsync(orgId, eventId);
            if (mails == null)
            {
                _logger.LogWarning("No mails found for event {EventId}", eventId);
                throw new NotFoundException("No Mails found");
            }

            return mails;
        }

        public async Task SendMailAsync(Guid orgId, Guid eventId, Guid mailId)
        {
            //var mail = await _mailRepository.GetMailByIdAsync(orgId, eventId, mailId);
            //if (mail == null)
            //{
            //    _logger.LogWarning("Mail {MailId} not found for event {EventId}", mailId, eventId);
            //    throw new NotFoundException("Mail not found");
            //}

            //using var smtpClient = new SmtpClient(_smtp.Host, _smtp.Port)
            //{
            //    Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
            //    EnableSsl = _smtp.EnableSsl
            //};

            //var mailTemplate = new MailMessage
            //{
            //    From = new MailAddress(_smtp.From),
            //    Subject = mail.Subject,
            //    Body = mail.Body,
            //    IsBodyHtml = true
            //};

            //if (mail.Recipients != null)
            //{
            //    foreach (var recipientId in mail.Recipients)
            //    {
            //        // Assuming you have a method to resolve recipient email by ID
            //        var recipientEmail = ResolveRecipientEmail(recipientId);
            //        if (!string.IsNullOrEmpty(recipientEmail))
            //        {
            //            mailTemplate.To.Add(recipientEmail);
            //        }
            //    }
            //}

            //smtpClient.Send(mailTemplate);
        }

        // Example helper method to resolve recipient email by ID
        private string ResolveRecipientEmail(Guid recipientId)
        {
            // Implement logic to resolve recipient email address based on ID
            // This could involve querying a database or another service
            return "example@example.com"; // Placeholder
        }

        public Task<bool> UpdateMailAsync(Guid orgId, Guid eventId, Guid mailId, CreateMailDto updateMailDto)
        {
            throw new NotImplementedException();
        }

        public async Task SendMailManualAsync(Guid orgId, Guid eventId, CreateMailDto mail, Guid userId)
        {
            // Empfänger bestimmen
            IEnumerable<Guid> recipientIds = mail.sendToAllParticipants
                ? (await _eventService.GetAllEventAttendeesAsync(orgId, eventId, userId)).Select(p => p.UserId)
                : mail.Recipients ?? throw new NotFoundException("No recipients found");

            if (!recipientIds.Any())
            {
                _logger.LogWarning("No recipients found");
                return;
            }

            // SMTP-Client initialisieren (einmal pro Aufruf)
            using var smtp = new SmtpClient();

            // Sicherheitseinstellungen (in Produktion proper validieren!)
            smtp.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await smtp.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_smtp.Username, _smtp.Password);

            try
            {
                foreach (var recipientId in recipientIds)
                {
                    try
                    {
                        var user = await _userService.GetUserByIdAsync(recipientId);
                        if (user?.Email == null) continue;

                        // Neue Nachricht für jeden Empfänger
                        var email = new MimeMessage();
                        email.From.Add(new MailboxAddress("NextGenDevelopment", _smtp.Username));
                        email.To.Add(new MailboxAddress($"{user.FirstName} {user.LastName}", user.Email));
                        email.Subject = mail.Subject;
                        email.Body = new TextPart("plain") { Text = mail.Body };

                        await smtp.SendAsync(email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending to recipient {recipientId}");
                        // Fortfahren mit nächstem Empfänger
                    }
                }
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
