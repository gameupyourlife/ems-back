using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Exceptions;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using Microsoft.Extensions.Logging;

namespace ems_back.Repo.Services
{
    public class MailService : IMailService
    {
        private readonly IMailRepository _mailRepository;
        private readonly IEventService _eventService;
        private readonly ILogger<MailService> _logger;

        public MailService(IMailRepository emailRepository, IEventService eventService, ILogger<MailService> logger)
        {
            _mailRepository = emailRepository;
            _eventService = eventService;
            _logger = logger;
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

        public Task<bool> UpdateMailAsync(Guid orgId, Guid eventId, Guid mailId, CreateMailDto updateMailDto)
        {
            throw new NotImplementedException();
        }
    }
}
