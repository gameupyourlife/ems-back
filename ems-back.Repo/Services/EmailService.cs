﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using Microsoft.Extensions.Logging;

namespace ems_back.Repo.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IEmailRepository emailRepository, ILogger<EmailService> logger)
        {
            _emailRepository = emailRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<EmailDto>> GetMailTemplates(Guid orgId)
        {
            throw new NotImplementedException();
        }

        public async Task<EmailDto> CreateMailTemplate(Guid orgId, EmailDto emailDto)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EmailDto>> GetEventMails(Guid orgId, Guid eventId)
        {
            var mails = await _emailRepository.GetEventMails(orgId, eventId);
            return mails;
        }

        public async Task<EmailDto> CreateEventMail(Guid orgId, Guid eventId, EmailDto emailDto)
        {
            throw new NotImplementedException();
        }        
    }
}
