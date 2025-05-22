using ems_back.Repo.DTOs.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Service
{
	public interface IMailTemplateService
	{
		Task<MailTemplateResponseDto> GetTemplateAsync(Guid id);
		Task<IEnumerable<MailTemplateResponseDto>> GetTemplatesForOrganizationAsync(Guid organizationId);
		Task<MailTemplateResponseDto> CreateTemplateAsync(Guid organizationId, Guid userId, CreateMailTemplateDto dto);
		Task<MailTemplateResponseDto> UpdateTemplateAsync(Guid id, Guid userId, UpdateMailTemplateDto dto);
		Task<bool> DeleteTemplateAsync(Guid id, Guid userId);
	}
}
