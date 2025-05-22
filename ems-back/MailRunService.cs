using ems_back.Repo.DTOs.Email.MailRun;
using ems_back.Repo.Interfaces.Service;

namespace ems_back
{
    public class MailRunService : IMailRunService
    {
        Task<MailRunDto> IMailRunService.CreateMailRunAsync(Guid orgId, Guid eventId, Guid mailId, CreateMailRunDto dto)
        {
            throw new NotImplementedException();
        }

        Task<bool> IMailRunService.DeleteMailRunAsync(Guid orgId, Guid eventId, Guid mailId, Guid mailRunId)
        {
            throw new NotImplementedException();
        }

        Task<MailRunDto> IMailRunService.GetMailRunByIdAsync(Guid orgId, Guid eventId, Guid mailId, Guid mailRunId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<MailRunDto>> IMailRunService.GetMailRunsForMailAsync(Guid orgId, Guid eventId, Guid mailId)
        {
            throw new NotImplementedException();
        }
    }
}