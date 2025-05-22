// Repository/MailTemplate/MailTemplateRepository.cs
using AutoMapper;
using ems_back.Repo.Data;
using ems_back.Repo.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ems_back.Repo.Repository.MailTemplate
{
	public class MailTemplateRepository : IMailRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		private readonly ILogger<MailTemplateRepository> _logger;

		public MailTemplateRepository(
			ApplicationDbContext context,
			IMapper mapper,
			ILogger<MailTemplateRepository> logger)
		{
			_context = context;
			_mapper = mapper;
			_logger = logger;
			_logger.LogInformation("MailTemplateRepository initialized");
		}

		public async Task<Models.MailTemplate> GetByIdAsync(Guid id)
		{
			_logger.LogDebug("Fetching mail template with ID: {TemplateId}", id);

			var template = await _context.MailTemplates
				.Include(t => t.Organization)
				.FirstOrDefaultAsync(t => t.Id == id);

			if (template == null)
			{
				_logger.LogWarning("Template not found with ID: {TemplateId}", id);
			}
			else
			{
				_logger.LogDebug("Successfully retrieved template with ID: {TemplateId}", id);
			}

			return template;
		}

		public async Task<IEnumerable<Models.MailTemplate>> GetByOrganizationAsync(Guid organizationId)
		{
			_logger.LogDebug("Fetching templates for organization ID: {OrganizationId}", organizationId);

			var templates = await _context.MailTemplates
				.Where(t => t.OrganizationId == organizationId)
				.Include(t => t.Organization)
				.ToListAsync();

			_logger.LogInformation("Retrieved {Count} templates for organization {OrganizationId}",
				templates.Count, organizationId);

			return templates;
		}

		public async Task<Models.MailTemplate> CreateAsync(Models.MailTemplate template)
		{
			_logger.LogDebug("Creating new mail template: {TemplateName}", template.Name);

			try
			{
				_context.MailTemplates.Add(template);
				await _context.SaveChangesAsync();

				_logger.LogInformation("Successfully created template with ID: {TemplateId}", template.Id);
				return template;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating mail template: {ErrorMessage}", ex.Message);
				throw;
			}
		}

		public async Task<Models.MailTemplate> UpdateAsync(Models.MailTemplate template)
		{
			_logger.LogDebug("Updating template with ID: {TemplateId}", template.Id);

			try
			{
				_context.MailTemplates.Update(template);
				await _context.SaveChangesAsync();

				_logger.LogInformation("Successfully updated template with ID: {TemplateId}", template.Id);
				return template;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating template {TemplateId}: {ErrorMessage}",
					template.Id, ex.Message);
				throw;
			}
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			_logger.LogDebug("Attempting to delete template with ID: {TemplateId}", id);

			var template = await _context.MailTemplates.FindAsync(id);
			if (template == null)
			{
				_logger.LogWarning("Delete failed - template not found with ID: {TemplateId}", id);
				return false;
			}

			try
			{
				_context.MailTemplates.Remove(template);
				var result = await _context.SaveChangesAsync() > 0;

				if (result)
				{
					_logger.LogInformation("Successfully deleted template with ID: {TemplateId}", id);
				}
				else
				{
					_logger.LogWarning("No changes made when deleting template with ID: {TemplateId}", id);
				}

				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting template {TemplateId}: {ErrorMessage}",
					id, ex.Message);
				throw;
			}
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			_logger.LogDebug("Checking existence of template with ID: {TemplateId}", id);
			var exists = await _context.MailTemplates.AnyAsync(t => t.Id == id);
			_logger.LogDebug("Template {TemplateId} exists: {Exists}", id, exists);
			return exists;
		}
	}
}