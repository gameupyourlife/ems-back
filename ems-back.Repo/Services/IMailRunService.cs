using ems_back.Repo.DTOs.Email.MailRun;
using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services
{
	public interface IMailRunService
	{
		Task<IEnumerable<MailRunDto>> GetMailRunsForMailAsync(Guid eventId, Guid mailId);
		Task<MailRunDto> GetMailRunForMailAsync(Guid eventId, Guid mailId, Guid runId);
		Task<MailRunDto> CreateMailRunAsync(CreateMailRunDto createMailRunDto);
		Task<bool> UpdateMailRunStatusAsync(Guid eventId, Guid mailId, Guid runId, MailRunStatus status);
	}
}
