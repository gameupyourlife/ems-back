using ems_back.Repo.Models;
using ems_back.Repo.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ems_back.Repo.Data;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Repositories
{
	public class MailTemplateRepository : IMailTemplateRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<MailTemplateRepository> _logger;

		public MailTemplateRepository(ApplicationDbContext context, ILogger<MailTemplateRepository> logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<MailTemplate> CreateAsync(MailTemplate template)
		{
			try
			{
				_logger.LogInformation("Creating new mail template with ID: {TemplateId}", template.Id);
				await _context.MailTemplates.AddAsync(template);
				await _context.SaveChangesAsync();
				return template;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating mail template");
				throw;
			}
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			try
			{
				_logger.LogInformation("Attempting to delete mail template with ID: {TemplateId}", id);
				var template = await _context.MailTemplates.FindAsync(id);
				if (template == null)
				{
					_logger.LogWarning("Mail template with ID {TemplateId} not found for deletion", id);
					return false;
				}

				_context.MailTemplates.Remove(template);
				await _context.SaveChangesAsync();
				_logger.LogInformation("Successfully deleted mail template with ID: {TemplateId}", id);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting mail template with ID: {TemplateId}", id);
				throw;
			}
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			try
			{
				_logger.LogDebug("Checking existence of mail template with ID: {TemplateId}", id);
				return await _context.MailTemplates.AnyAsync(t => t.Id == id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error checking existence of mail template with ID: {TemplateId}", id);
				throw;
			}
		}

		public async Task<MailTemplate> GetByIdAsync(Guid id)
		{
			try
			{
				_logger.LogDebug("Fetching mail template with ID: {TemplateId}", id);
				return await _context.MailTemplates.FindAsync(id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching mail template with ID: {TemplateId}", id);
				throw;
			}
		}

		public async Task<IEnumerable<MailTemplate>> GetByOrganizationAsync(Guid organizationId)
		{
			try
			{
				_logger.LogDebug("Fetching mail templates for organization ID: {OrganizationId}", organizationId);
				return await _context.MailTemplates
					.Where(t => t.OrganizationId == organizationId)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching mail templates for organization ID: {OrganizationId}", organizationId);
				throw;
			}
		}

		public async Task<MailTemplate> UpdateAsync(MailTemplate template)
		{
			try
			{
				_logger.LogInformation("Updating mail template with ID: {TemplateId}", template.Id);
				_context.MailTemplates.Update(template);
				await _context.SaveChangesAsync();
				return template;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating mail template with ID: {TemplateId}", template.Id);
				throw;
			}
		}
	}
}