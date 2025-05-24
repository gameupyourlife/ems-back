using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models.Types;
using Microsoft.Extensions.Logging;

namespace ems_back.Controllers
{
	[ApiController]
	[Route("api/org/{orgId}/mail-templates")]
	[Authorize]
	public class MailTemplatesController : ControllerBase
	{
		private readonly IMailTemplateService _mailTemplateService;
		private readonly ILogger<MailTemplatesController> _logger;

		public MailTemplatesController(
			IMailTemplateService mailTemplateService,
			ILogger<MailTemplatesController> logger)
		{
			_mailTemplateService = mailTemplateService ?? throw new ArgumentNullException(nameof(mailTemplateService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			_logger.LogInformation("MailTemplatesController initialized");
		}

		private Guid GetUserIdFromClaims()
		{
			try
			{
				var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
				if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
				{
					_logger.LogError("User ID not found in claims or invalid format");
					throw new UnauthorizedAccessException("User identification failed");
				}
				return userId;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error extracting user ID from claims");
				throw;
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<IEnumerable<MailDto>>> GetTemplates([FromRoute] Guid orgId)
		{
			try
			{
				_logger.LogDebug("Fetching mail templates for organization {OrganizationId}", orgId);
				var templates = await _mailTemplateService.GetTemplatesForOrganizationAsync(orgId);

				_logger.LogInformation("Successfully retrieved {Count} templates for organization {OrganizationId}",
					templates.Count(), orgId);

				return Ok(templates);
			}
			catch (KeyNotFoundException ex)
			{
				_logger.LogWarning(ex, "Organization {OrganizationId} not found", orgId);
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching templates for organization {OrganizationId}", orgId);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<MailDto>> GetTemplate([FromRoute] Guid orgId, [FromRoute] Guid id)
		{
			try
			{
				_logger.LogDebug("Fetching mail template {TemplateId} for organization {OrganizationId}", id, orgId);
				var template = await _mailTemplateService.GetTemplateAsync(id);

				if (template == null)
				{
					_logger.LogWarning("Template {TemplateId} not found in organization {OrganizationId}", id, orgId);
					return NotFound();
				}

				return Ok(template);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching template {TemplateId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		[HttpPost]
		[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)},{nameof(UserRole.Organizer)},{nameof(UserRole.EventOrganizer)}")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<MailDto>> CreateTemplate(
			[FromRoute] Guid orgId,
			[FromBody] CreateMailDto dto)
		{
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid model state for creating mail template");
				return BadRequest(ModelState);
			}

			try
			{
				var userId = GetUserIdFromClaims();
				_logger.LogInformation("Creating new mail template for organization {OrganizationId} by user {UserId}",
					orgId, userId);

				var createdTemplate = await _mailTemplateService.CreateTemplateAsync(orgId, userId, dto);

				_logger.LogInformation("Successfully created mail template {TemplateId}", createdTemplate.Id);

				return CreatedAtAction(
					nameof(GetTemplate),
					new { orgId, id = createdTemplate.Id },
					createdTemplate);
			}
			catch (UnauthorizedAccessException ex)
			{
				_logger.LogWarning(ex, "Unauthorized attempt to create mail template");
				return Unauthorized(ex.Message);
			}
			catch (KeyNotFoundException ex)
			{
				_logger.LogWarning(ex, "Organization {OrganizationId} not found", orgId);
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating mail template for organization {OrganizationId}", orgId);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		[HttpPut("{id}")]
		[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)},{nameof(UserRole.Organizer)},{nameof(UserRole.EventOrganizer)}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<MailDto>> UpdateTemplate(
			[FromRoute] Guid orgId,
			[FromRoute] Guid id,
			[FromBody] CreateMailDto dto)
		{
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid model state for updating mail template {TemplateId}", id);
				return BadRequest(ModelState);
			}

			try
			{
				var userId = GetUserIdFromClaims();
				_logger.LogInformation("Updating mail template {TemplateId} by user {UserId}", id, userId);

				var updatedTemplate = await _mailTemplateService.UpdateTemplateAsync(id, userId, dto);

				_logger.LogInformation("Successfully updated mail template {TemplateId}", id);

				return Ok(updatedTemplate);
			}
			catch (UnauthorizedAccessException ex)
			{
				_logger.LogWarning(ex, "Unauthorized attempt to update mail template {TemplateId}", id);
				return Unauthorized(ex.Message);
			}
			catch (KeyNotFoundException ex)
			{
				_logger.LogWarning(ex, "Template {TemplateId} not found for update", id);
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating template {TemplateId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)},{nameof(UserRole.Organizer)},{nameof(UserRole.EventOrganizer)}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteTemplate([FromRoute] Guid orgId, [FromRoute] Guid id)
		{
			try
			{
				var userId = GetUserIdFromClaims();
				_logger.LogInformation("Deleting mail template {TemplateId} by user {UserId}", id, userId);

				var deleted = await _mailTemplateService.DeleteTemplateAsync(id, userId);

				if (!deleted)
				{
					_logger.LogWarning("Template {TemplateId} not found for deletion", id);
					return NotFound();
				}

				_logger.LogInformation("Successfully deleted mail template {TemplateId}", id);
				return NoContent();
			}
			catch (UnauthorizedAccessException ex)
			{
				_logger.LogWarning(ex, "Unauthorized attempt to delete mail template {TemplateId}", id);
				return Unauthorized(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting template {TemplateId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}
	}
}