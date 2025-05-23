using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Repository
{
    public interface IMailRepository
    {
        Task<Guid> CreateMailAsync(Guid eventId, MailDto createMailDto);
        Task<bool> DeleteMailAsync(Guid orgId, Guid eventId, Guid mailId);
        Task<MailDto> GetMailByIdAsync(Guid orgId, Guid eventId, Guid mailId);
        Task<IEnumerable<MailDto>> GetMailsForEventAsync(Guid orgId, Guid eventId);
        Task<MailDto> UpdateMailAsync(Guid orgId, Guid eventId, Guid mailId, CreateMailDto mailDto, Guid userId);
    }
}
