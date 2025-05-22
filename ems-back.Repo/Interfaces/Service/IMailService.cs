using ems_back.Repo.DTOs.Email.MailRun;
using ems_back.Repo.DTOs.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Service
{
	public interface IMailService
	{

		Task<IEnumerable<MailDto>> GetMailsForEventAsync(Guid orgId, Guid eventId);
		Task<MailDto> GetMailByIdAsync(Guid orgId, Guid eventId, Guid mailId);
		Task<MailDto> CreateMailAsync(Guid orgId, Guid eventId, CreateMailDto createMailDto);
		Task<bool> UpdateMailAsync(Guid orgId, Guid eventId, Guid mailId, UpdateMailDto updateMailDto);
		Task<bool> DeleteMailAsync(Guid orgId, Guid eventId, Guid mailId);
	}
}
