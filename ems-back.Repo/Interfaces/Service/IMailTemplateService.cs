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
        Task<MailDto> GetTemplateAsync(Guid id);
        Task<IEnumerable<MailDto>> GetTemplatesForOrganizationAsync(Guid organizationId);
        Task<MailDto> CreateTemplateAsync(Guid organizationId, Guid userId, CreateMailDto dto);
        Task<MailDto> UpdateTemplateAsync(Guid id, Guid userId, CreateMailDto dto);
        Task<bool> DeleteTemplateAsync(Guid id, Guid userId);
    }
}