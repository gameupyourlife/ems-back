using ems_back.Emails;
using ems_back.Repo.Data;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Jobs.Mail
{
    public class MailQueueWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MailQueueWorker> _logger;
        private readonly SmtpSettings _smtp;

        public MailQueueWorker(
            IServiceScopeFactory scopeFactory, 
            ILogger<MailQueueWorker> logger,
            IOptions<SmtpSettings> options)
        {
            _smtp = options.Value;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var mails = await db.MailQueueEntries
                    .Where(m => m.Status == MailRunStatus.Pending && m.RetryCount < 5)
                    .OrderBy(m => m.CreatedAt)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                foreach (var mail in mails)
                {
                    try
                    {
                        await SendEmailAsync(mail);
                        mail.Status = MailRunStatus.Sent;
                    }
                    catch (Exception ex)
                    {
                        mail.Status = MailRunStatus.Pending;
                        mail.RetryCount++;
                        _logger.LogError(ex, "Failed to send mail to {Email}", mail.ToEmail);
                    }

                    db.Update(mail);
                }

                await db.SaveChangesAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task SendEmailAsync(MailQueueEntry mail)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtp.Company, _smtp.Username));
            message.To.Add(new MailboxAddress(mail.ToName, mail.ToEmail));
            message.Subject = mail.Subject;
            message.Body = new TextPart("plain") { Text = mail.Body };

            _logger.LogInformation("Sending email to {Email}", mail.ToEmail);

            using var smtp = new SmtpClient();

            smtp.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await smtp.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.SslOnConnect);
            await smtp.AuthenticateAsync(_smtp.Username, _smtp.Password);
            await smtp.SendAsync(message);
            _logger.LogInformation("Email sent to {Email}", mail.ToEmail);
            await smtp.DisconnectAsync(true);
        }
    }
}
