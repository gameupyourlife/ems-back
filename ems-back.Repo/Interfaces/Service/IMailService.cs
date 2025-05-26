using ems_back.Repo.DTOs.Email.MailRun;
using ems_back.Repo.DTOs.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Mail;

namespace ems_back.Repo.Interfaces.Service
{
	public interface IMailService
	{

		Task<IEnumerable<MailDto>> GetMailsForEventAsync(Guid orgId, Guid eventId, Guid userId);
		Task<MailDto> GetMailByIdAsync(Guid orgId, Guid eventId, Guid mailId, Guid userId);
		Task<MailDto> CreateMailAsync(Guid orgId, Guid eventId, CreateMailDto createMailDto, Guid userId);
		Task<MailDto> UpdateMailAsync(Guid orgId, Guid eventId, Guid mailId, CreateMailDto updateMailDto, Guid userId);
		Task<bool> DeleteMailAsync(Guid orgId, Guid eventId, Guid mailId, Guid userId);
        Task SendMailByIdAsync(Guid orgId, Guid eventId, Guid mailId, Guid userId);
        Task SendMailWithDtoAsync(Guid orgId, Guid eventId, CreateMailDto sendMailManualDto, Guid userId);
		Task SendMailManualAsync(Guid orgId, MailManualDto manualDto, Guid userId);
        Task<bool> ExistsOrg(Guid orgId);
		Task SendMailByFlowAsync(Guid orgId, Guid eventId, Guid mailId);
    }
}
