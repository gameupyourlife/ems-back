using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models.Types;

namespace ems_back.Controllers
{
	[ApiController]
	[Route("api/orgs/{organizationId}/[controller]")]
	[Authorize]
	public class MailTemplatesController : ControllerBase
	{
		private readonly IMailTemplateService _mailTemplateService;
		private readonly ILogger<MailTemplatesController> _logger;

		public MailTemplatesController(
			IMailTemplateService mailTemplateService,
			ILogger<MailTemplatesController> logger)
		{
			_mailTemplateService = mailTemplateService;
			_logger = logger;
			_logger.LogInformation("MailTemplatesController initialized");
		}

		private Guid GetUserIdFromClaims()
		{
			var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
			if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
			{
				_logger.LogError("User ID not found in claims");
				throw new UnauthorizedAccessException("User identification failed");
			}
			return userId;
		}

		/// <summary>
		/// Get all mail templates for an organization
		/// </summary>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<IEnumerable<MailTemplateResponseDto>>> GetTemplates(
			[FromRoute] Guid organizationId)
		{
			_logger.LogDebug("Getting all templates for organization {OrganizationId}", organizationId);

			try
			{
				var templates = await _mailTemplateService.GetTemplatesForOrganizationAsync(organizationId);
				_logger.LogInformation("Returning {Count} templates for organization {OrganizationId}",
					templates.Count(), organizationId);
				return Ok(templates);
			}
			catch (KeyNotFoundException ex)
			{
				_logger.LogWarning(ex, "Organization not found: {OrganizationId}", organizationId);
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting templates for organization {OrganizationId}", organizationId);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}


		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<MailTemplateResponseDto>> GetTemplate(
			[FromRoute] Guid organizationId,
			[FromRoute] Guid id)
		{
			_logger.LogDebug("Getting template {TemplateId} for organization {OrganizationId}",
				id, organizationId);

			try
			{
				var template = await _mailTemplateService.GetTemplateAsync(id);

				if (template == null || template.OrganizationId != organizationId)
				{
					_logger.LogWarning("Template {TemplateId} not found in organization {OrganizationId}",
						id, organizationId);
					return NotFound();
				}

				return Ok(template);
			}
			catch (KeyNotFoundException ex)
			{
				_logger.LogWarning(ex, "Template not found: {TemplateId}", id);
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting template {TemplateId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}
		[HttpPost]
		[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)},{nameof(UserRole.Organizer)},{nameof(UserRole.EventOrganizer)}")]
		public async Task<ActionResult<MailTemplateResponseDto>> CreateTemplate(
			[FromRoute] Guid organizationId,
			[FromBody] CreateMailTemplateDto dto)
		{
			_logger.LogDebug("Creating new template for organization {OrganizationId}", organizationId);

			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid model state for template creation");
				return BadRequest(ModelState);
			}

			try
			{
				// Get user ID from claims
				var userId = GetUserIdFromClaims();
				if (userId == Guid.Empty)
				{
					_logger.LogWarning("Invalid user ID in claims");
					return Unauthorized("Invalid user identification");
				}

				var createdTemplate = await _mailTemplateService.CreateTemplateAsync(organizationId, userId, dto);

				_logger.LogInformation("Created new template {TemplateId} for organization {OrganizationId}",
					createdTemplate.Id, organizationId);

				return CreatedAtAction(
					nameof(GetTemplate),
					new { organizationId, id = createdTemplate.Id },
					createdTemplate);
			}
			catch (UnauthorizedAccessException ex)
			{
				_logger.LogWarning(ex, "Authorization failed");
				return Unauthorized(ex.Message);
			}
			catch (KeyNotFoundException ex)
			{
				_logger.LogWarning(ex, "Organization not found: {OrganizationId}", organizationId);
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating template for organization {OrganizationId}", organizationId);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		[HttpPut("{id}")]
		[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)},{nameof(UserRole.Organizer)},{nameof(UserRole.EventOrganizer)}")]
		public async Task<ActionResult<MailTemplateResponseDto>> UpdateTemplate(
			[FromRoute] Guid organizationId,
			[FromRoute] Guid id,
			[FromBody] UpdateMailTemplateDto dto)
		{
			_logger.LogDebug("Updating template {TemplateId} for organization {OrganizationId}",
				id, organizationId);

			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid model state for template update");
				return BadRequest(ModelState);
			}

			try
			{
				var userId = GetUserIdFromClaims();
				if (userId == Guid.Empty)
				{
					_logger.LogWarning("Invalid user ID in claims");
					return Unauthorized("Invalid user identification");
				}

				var updatedTemplate = await _mailTemplateService.UpdateTemplateAsync(id, userId, dto);

				if (updatedTemplate.OrganizationId != organizationId)
				{
					_logger.LogWarning("Template {TemplateId} does not belong to organization {OrganizationId}",
						id, organizationId);
					return NotFound();
				}

				_logger.LogInformation("Updated template {TemplateId}", id);
				return Ok(updatedTemplate);
			}
			catch (UnauthorizedAccessException ex)
			{
				_logger.LogWarning(ex, "Authorization failed");
				return Unauthorized(ex.Message);
			}
			catch (KeyNotFoundException ex)
			{
				_logger.LogWarning(ex, "Template not found: {TemplateId}", id);
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating template {TemplateId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}
		//needs to be check not working fully.
		[HttpDelete("{id}")]
		[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)},{nameof(UserRole.Organizer)},{nameof(UserRole.EventOrganizer)}")]
		public async Task<IActionResult> DeleteTemplate(
			[FromRoute] Guid organizationId,
			[FromRoute] Guid id)
		{
			_logger.LogDebug("Deleting template {TemplateId} from organization {OrganizationId}",
				id, organizationId);

			try
			{
				var userId = GetUserIdFromClaims();
				if (userId == Guid.Empty)
				{
					_logger.LogWarning("Invalid user ID in claims");
					return Unauthorized("Invalid user identification");
				}

				var template = await _mailTemplateService.GetTemplateAsync(id);
				if (template == null || template.OrganizationId != organizationId)
				{
					_logger.LogWarning("Template {TemplateId} not found in organization {OrganizationId}",
						id, organizationId);
					return NotFound();
				}


				var result = await _mailTemplateService.DeleteTemplateAsync(id, userId);
				if (!result)
				{
					_logger.LogWarning("Delete failed for template {TemplateId}", id);
					return NotFound();
				}

				_logger.LogInformation("Deleted template {TemplateId}", id);
				return NoContent();
			}
			catch (UnauthorizedAccessException ex)
			{
				_logger.LogWarning(ex, "Authorization failed");
				return Unauthorized(ex.Message);
			}
			catch (KeyNotFoundException ex)
			{
				_logger.LogWarning(ex, "Template not found: {TemplateId}", id);
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting template {TemplateId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}
	}
}