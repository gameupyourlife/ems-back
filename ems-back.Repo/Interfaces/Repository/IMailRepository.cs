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
        Task<IEnumerable<MailDto>> GetMailsForEventAsync(Guid orgId, Guid eventId);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
