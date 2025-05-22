using AutoMapper;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services
{
	// Service/MailTemplate/MailTemplateService.cs
	namespace ems_back.Repo.Service.MailTemplate
	{
		public class MailTemplateService : IMailTemplateService
		{
			private readonly IMailRepository _repository;
			private readonly IMapper _mapper;
			private readonly IOrganizationService _organizationService;
			private readonly IOrganizationRepository _organizationRepository;
			private readonly UserManager<User> _userManager;

			private readonly ILogger<MailTemplateService> _logger;

			public MailTemplateService(
				IMailRepository repository,
				IMapper mapper,
				UserManager<User> userManager,
				IOrganizationService organizationService,
				IOrganizationRepository organizationRepository,
			ILogger<MailTemplateService> logger)
			{
				_repository = repository;
				_mapper = mapper;
				_organizationService = organizationService;
				_logger = logger;
				_userManager = userManager;

				_organizationRepository = organizationRepository;
				_logger.LogInformation("MailTemplateService initialized");
			}

            Task<MailDto> IMailTemplateService.CreateTemplateAsync(Guid organizationId, Guid userId, CreateMailDto dto)
            {
                throw new NotImplementedException();
            }

            Task<bool> IMailTemplateService.DeleteTemplateAsync(Guid id, Guid userId)
            {
                throw new NotImplementedException();
            }

            Task<MailDto> IMailTemplateService.GetTemplateAsync(Guid id)
            {
                throw new NotImplementedException();
            }

            Task<IEnumerable<MailDto>> IMailTemplateService.GetTemplatesForOrganizationAsync(Guid organizationId)
            {
                throw new NotImplementedException();
            }

            Task<MailDto> IMailTemplateService.UpdateTemplateAsync(Guid id, Guid userId, CreateMailDto dto)
            {
                throw new NotImplementedException();
            }
        }
	}
}
