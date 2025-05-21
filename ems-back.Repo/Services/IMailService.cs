using ems_back.Repo.DTOs.Email.MailRun;
using ems_back.Repo.DTOs.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services
{
	public interface IMailService
	{
		Task<IEnumerable<EmailDto>> GetMailsForEventAsync(Guid eventId);
		Task<EmailDto> GetMailForEventAsync(Guid eventId, Guid mailId);
		Task<EmailDto> CreateMailAsync(CreateMailDto createMailDto);
		Task<bool> UpdateMailForEventAsync(Guid eventId, Guid mailId, UpdateMailDto updateMailDto);
		Task<bool> DeleteMailForEventAsync(Guid eventId, Guid mailId);
		Task<IEnumerable<MailRunDto>> GetMailRunsForMailAsync(Guid eventId, Guid mailId);
	}
}
