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


			public async Task<MailDto> GetTemplateAsync(Guid id)
			{
				_logger.LogDebug("Service layer - Getting template with ID: {TemplateId}", id);

				var template = await _repository.GetByIdAsync(id);
				if (template == null)
				{
					_logger.LogWarning("Template not found in service layer: {TemplateId}", id);
					throw new KeyNotFoundException($"Template with ID {id} not found");
				}

				_logger.LogDebug("Successfully mapped template {TemplateId} to DTO", id);
				return _mapper.Map<MailDto>(template);
			}

			public async Task<IEnumerable<MailDto>> GetTemplatesForOrganizationAsync(Guid organizationId)
			{
				_logger.LogDebug("Getting templates for organization: {OrganizationId}", organizationId);

				//check if Org exists
				if (!await _organizationRepository.OrganizationExistsAsync(organizationId))
				{
					_logger.LogWarning("Organization not found: {OrganizationId}", organizationId);
					throw new KeyNotFoundException("Organization not found");
				}
				
				var templates = await _repository.GetByOrganizationAsync(organizationId);
				_logger.LogInformation("Retrieved {Count} templates for organization {OrganizationId}",
					templates.Count(), organizationId);

				return _mapper.Map<IEnumerable<MailDto>>(templates);
			}

			public async Task<MailDto> CreateTemplateAsync(
				Guid organizationId,
				Guid userId,
				CreateMailDto dto)
			{
				_logger.LogDebug("Creating new template for organization {OrganizationId}", organizationId);

				// Get user first to avoid multiple DB calls
				var user = await _userManager.FindByIdAsync(userId.ToString());
				if (user == null)
				{
					_logger.LogWarning("User {UserId} not found", userId);
					throw new UnauthorizedAccessException("User not found");
				}

				if (!await _organizationRepository.OrganizationExistsAsync(organizationId))
				{
					_logger.LogError("Create failed - organization not found: {OrganizationId}", organizationId);
					throw new KeyNotFoundException("Organization not found");
				}

				// Check rights - more efficient single call
				var userRoles = await _userManager.GetRolesAsync(user);

				// Permission logic:
				// Allow if user is Admin, Owner, Organizer, or EventOrganizer
				var hasPermission = userRoles.Contains(nameof(UserRole.Admin)) ||
								   userRoles.Contains(nameof(UserRole.Owner)) ||
								   userRoles.Contains(nameof(UserRole.Organizer)) ||
								   userRoles.Contains(nameof(UserRole.EventOrganizer));

				if (!hasPermission)
				{
					_logger.LogWarning("User {UserId} with roles {UserRoles} lacks permission to create templates",
						userId, string.Join(",", userRoles));
					throw new UnauthorizedAccessException("Insufficient permissions");
				}

				var template = _mapper.Map<Models.MailTemplate>(dto);
				template.OrganizationId = organizationId;
				template.isUserCreated = true;


				_logger.LogInformation("Creating template: {TemplateName} by user {UserId}",
					template.Name, userId);

				var created = await _repository.CreateAsync(template);
				_logger.LogDebug("Template created with ID: {TemplateId}", created.Id);

				return _mapper.Map<MailDto>(created);
			}
			public async Task<MailDto> UpdateTemplateAsync(
				Guid id,
				Guid userId,
				CreateMailDto dto)
			{
				//

				_logger.LogDebug("Updating template {TemplateId} by user {UserId}", id, userId);

				var user = await _userManager.FindByIdAsync(userId.ToString());
				if (user == null)
				{
					_logger.LogWarning("User {UserId} not found", userId);
					throw new UnauthorizedAccessException("User not found");
				}

				// Check rights - more efficient single call
				var userRoles = await _userManager.GetRolesAsync(user);

				// Permission logic:
				// Allow if user is Admin, Owner, Organizer, or EventOrganizer
				var hasPermission = userRoles.Contains(nameof(UserRole.Admin)) ||
				                    userRoles.Contains(nameof(UserRole.Owner)) ||
				                    userRoles.Contains(nameof(UserRole.Organizer)) ||
				                    userRoles.Contains(nameof(UserRole.EventOrganizer));

				if (!hasPermission)
				{
					_logger.LogWarning("User {UserId} with roles {UserRoles} lacks permission to update templates",
						userId, string.Join(",", userRoles));
					throw new UnauthorizedAccessException("Insufficient permissions");
				}

				var existing = await _repository.GetByIdAsync(id);
				if (existing == null)
				{
					_logger.LogError("Update failed - template not found: {TemplateId}", id);
					throw new KeyNotFoundException("Template not found");
				}

				_logger.LogDebug("Mapping update DTO to existing template {TemplateId}", id);
				_mapper.Map(dto, existing);


				_logger.LogInformation("Saving updates to template {TemplateId}", id);
				var updated = await _repository.UpdateAsync(existing);

				return _mapper.Map<MailDto>(updated);
			}

			public async Task<bool> DeleteTemplateAsync(Guid id, Guid userId
			)
			{
				_logger.LogDebug("Attempting to delete template {TemplateId}", id);


				_logger.LogDebug("Updating template {TemplateId} by user {UserId}", id, userId);

				var user = await _userManager.FindByIdAsync(userId.ToString());
				if (user == null)
				{
					_logger.LogWarning("User {UserId} not found", userId);
					throw new UnauthorizedAccessException("User not found");
				}

				// Check rights - more efficient single call
				var userRoles = await _userManager.GetRolesAsync(user);

				// Permission logic:
				// Allow if user is Admin, Owner, Organizer, or EventOrganizer
				var hasPermission = userRoles.Contains(nameof(UserRole.Admin)) ||
				                    userRoles.Contains(nameof(UserRole.Owner)) ||
				                    userRoles.Contains(nameof(UserRole.Organizer)) ||
				                    userRoles.Contains(nameof(UserRole.EventOrganizer));

				if (!hasPermission)
				{
					_logger.LogWarning("User {UserId} with roles {UserRoles} lacks permission to update templates",
						userId, string.Join(",", userRoles));
					throw new UnauthorizedAccessException("Insufficient permissions");
				}

				if (!await _repository.ExistsAsync(id))
				{
					_logger.LogError("Delete failed - template not found: {TemplateId}", id);
					throw new KeyNotFoundException("Template not found");
				}

				var result = await _repository.DeleteAsync(id);
				_logger.LogInformation("Delete operation for template {TemplateId} completed: {Result}",
					id, result);

				return result;
			}
		}
	}
}
