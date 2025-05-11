using ems_back.Repo.DTOs.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Repository
{
    public interface IEmailRepository
    {
        Task<IEnumerable<EmailDto>> GetMailTemplates(Guid orgId);
        Task<EmailDto> CreateMailTemplate(Guid orgId, EmailDto emailDto);
        Task<IEnumerable<EmailDto>> GetEventMails(Guid orgId, Guid eventId);
        Task<EmailDto> CreateEventMail(Guid orgId, Guid eventId, EmailDto emailDto);
    }
}
