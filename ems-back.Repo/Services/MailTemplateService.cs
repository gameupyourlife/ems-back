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
using System.Threading.Tasks;

namespace ems_back.Repo.Services
{
    public class MailTemplateService : IMailTemplateService
    {
        private readonly IMailTemplateRepository _repository;
        private readonly IMapper _mapper;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<MailTemplateService> _logger;

        public MailTemplateService(
            IMailTemplateRepository repository,
            IMapper mapper,
            UserManager<User> userManager,
            IOrganizationRepository organizationRepository,
            ILogger<MailTemplateService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation("MailTemplateService initialized");
        }

        public async Task<MailDto> GetTemplateAsync(Guid id)
        {
            try
            {
                _logger.LogDebug("Fetching mail template {TemplateId}", id);
                var template = await _repository.GetByIdAsync(id);

                if (template == null)
                {
                    _logger.LogWarning("Mail template {TemplateId} not found", id);
                    throw new KeyNotFoundException($"Template with ID {id} not found");
                }

                return _mapper.Map<MailDto>(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching mail template {TemplateId}", id);
                throw;
            }
        }
        public async Task<IEnumerable<MailDto>> GetTemplatesForOrganizationAsync(Guid organizationId)
        {
            try
            {
                _logger.LogDebug("Fetching mail templates for organization {OrganizationId}", organizationId);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching mail templates for organization {OrganizationId}", organizationId);
                throw;
            }
        }
        public async Task<MailDto> CreateTemplateAsync(Guid organizationId, Guid userId, CreateMailDto dto)
        {
            try
            {
                _logger.LogDebug("Creating new template for organization {OrganizationId} by user {UserId}",
                    organizationId, userId);

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

                var userRoles = await _userManager.GetRolesAsync(user);
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

                var template = _mapper.Map<MailTemplate>(dto);
                template.Id = Guid.NewGuid();
                template.OrganizationId = organizationId;
                template.CreatedBy = userId;
                template.CreatedAt = DateTime.UtcNow;
                template.isUserCreated = true;

                _logger.LogInformation("Creating template: {TemplateName} by user {UserId}",
                    template.Name, userId);

                var createdTemplate = await _repository.CreateAsync(template);
                _logger.LogDebug("Template created with ID: {TemplateId}", createdTemplate.Id);

                return _mapper.Map<MailDto>(createdTemplate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating mail template for organization {OrganizationId} by user {UserId}",
                    organizationId, userId);
                throw;
            }
        }

        public async Task<MailDto> UpdateTemplateAsync(Guid id, Guid userId, CreateMailDto dto)
        {
            try
            {
                _logger.LogDebug("Updating template {TemplateId} by user {UserId}", id, userId);

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    throw new UnauthorizedAccessException("User not found");
                }

                var userRoles = await _userManager.GetRolesAsync(user);
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

                var existingTemplate = await _repository.GetByIdAsync(id);
                if (existingTemplate == null)
                {
                    _logger.LogError("Update failed - template not found: {TemplateId}", id);
                    throw new KeyNotFoundException("Template not found");
                }

                _mapper.Map(dto, existingTemplate);
                existingTemplate.UpdatedBy = userId;
                existingTemplate.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Saving updates to template {TemplateId}", id);
                var updatedTemplate = await _repository.UpdateAsync(existingTemplate);

                return _mapper.Map<MailDto>(updatedTemplate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating mail template {TemplateId} by user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<bool> DeleteTemplateAsync(Guid id, Guid userId)
        {
            try
            {
                _logger.LogDebug("Attempting to delete template {TemplateId} by user {UserId}", id, userId);

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    throw new UnauthorizedAccessException("User not found");
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var hasPermission = userRoles.Contains(nameof(UserRole.Admin)) ||
                                   userRoles.Contains(nameof(UserRole.Owner)) ||
                                   userRoles.Contains(nameof(UserRole.Organizer)) ||
                                   userRoles.Contains(nameof(UserRole.EventOrganizer));

                if (!hasPermission)
                {
                    _logger.LogWarning("User {UserId} with roles {UserRoles} lacks permission to delete templates",
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting template {TemplateId} by user {UserId}", id, userId);
                throw;
            }
        }
    }
}