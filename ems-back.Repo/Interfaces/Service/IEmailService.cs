using ems_back.Repo.DTOs.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Service
{
    public interface IEmailService
    {
        Task<IEnumerable<MailDto>> GetMailTemplates(Guid orgId);
        Task<MailDto> CreateMailTemplate(Guid orgId, MailDto mailDto);
        Task<IEnumerable<MailDto>> GetEventMails(Guid orgId, Guid eventId);
        Task<MailDto> CreateEventMail(Guid orgId, Guid eventId, MailDto mailDto);
    }
}