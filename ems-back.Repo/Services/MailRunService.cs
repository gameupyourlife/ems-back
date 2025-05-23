using ems_back.Repo.DTOs.Email.MailRun;
using ems_back.Repo.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services
{
    public class MailRunService : IMailRunService
    {
        public Task<MailRunDto> CreateMailRunAsync(Guid orgId, Guid eventId, Guid mailId, CreateMailRunDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteMailRunAsync(Guid orgId, Guid eventId, Guid mailId, Guid mailRunId)
        {
            throw new NotImplementedException();
        }

        public Task<MailRunDto> GetMailRunByIdAsync(Guid orgId, Guid eventId, Guid mailId, Guid mailRunId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MailRunDto>> GetMailRunsForMailAsync(Guid orgId, Guid eventId, Guid mailId)
        {
            throw new NotImplementedException();
        }
    }
}
