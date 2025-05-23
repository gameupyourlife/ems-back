using ems_back.Repo.DTOs.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Repository
{
    public interface IMailRepository
    {
        Task<MailDto> GetMailByIdAsync(Guid orgId, Guid eventId, Guid mailId);
        Task<IEnumerable<MailDto>> GetMailsForEventAsync(Guid orgId, Guid eventId);
    }
}
