using ems_back.Repo.DTOs.Email.MailRun;
using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Service
{
	public interface IMailRunService
	{
		Task<IEnumerable<MailRunDto>> GetMailRunsForMailAsync(Guid orgId, Guid eventId, Guid mailId);
		Task<MailRunDto> GetMailRunByIdAsync(Guid orgId, Guid eventId, Guid mailId, Guid mailRunId);
		Task<MailRunDto> CreateMailRunAsync(Guid orgId, Guid eventId, Guid mailId, CreateMailRunDto dto);
		Task<bool> DeleteMailRunAsync(Guid orgId, Guid eventId, Guid mailId, Guid mailRunId);
	}
}
