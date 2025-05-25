using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Repository
{
    public interface IMailTemplateRepository
    {
        Task<MailTemplate> GetByIdAsync(Guid id);
        Task<IEnumerable<MailTemplate>> GetByOrganizationAsync(Guid organizationId);
        Task<MailTemplate> CreateAsync(MailTemplate template);
        Task<MailTemplate> UpdateAsync(MailTemplate template);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
