using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models.Types;

namespace ems_back.Controllers
{
	[ApiController]
	[Route("api/org/{orgId}/mail-templates")]
	[Authorize]
	public class MailTemplatesController : ControllerBase
	{
		private readonly IMailTemplateService _mailTemplateService;
		private readonly ILogger<MailTemplatesController> _logger;

		public MailTemplatesController(IMailTemplateService mailTemplateService, ILogger<MailTemplatesController> logger)
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

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<MailTemplateResponseDto>>> GetTemplates([FromRoute] Guid orgId)
		{
			try
			{
				var templates = await _mailTemplateService.GetTemplatesForOrganizationAsync(orgId);
				_logger.LogInformation("Returned {Count} templates for org {OrgId}", templates.Count(), orgId);
				return Ok(templates);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching templates for org {OrgId}", orgId);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<MailTemplateResponseDto>> GetTemplate([FromRoute] Guid orgId, [FromRoute] Guid id)
		{
			try
			{
				var template = await _mailTemplateService.GetTemplateAsync(id);
				if (template == null)
				{
					_logger.LogWarning("Template {TemplateId} not found in org {OrgId}", id, orgId);
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
		public async Task<ActionResult<MailTemplateResponseDto>> CreateTemplate([FromRoute] Guid orgId, [FromBody] CreateMailTemplateDto dto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var userId = GetUserIdFromClaims();
				var createdTemplate = await _mailTemplateService.CreateTemplateAsync(orgId, userId, dto);

				return CreatedAtAction(nameof(GetTemplate), new { orgId, id = createdTemplate.Id }, createdTemplate);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating template in org {OrgId}", orgId);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		[HttpPut("{id}")]
		[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)},{nameof(UserRole.Organizer)},{nameof(UserRole.EventOrganizer)}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<MailTemplateResponseDto>> UpdateTemplate([FromRoute] Guid orgId, [FromRoute] Guid id, [FromBody] UpdateMailTemplateDto dto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var userId = GetUserIdFromClaims();
				var updated = await _mailTemplateService.UpdateTemplateAsync(id, userId, dto);
				return Ok(updated);
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
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
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteTemplate([FromRoute] Guid orgId, [FromRoute] Guid id)
		{
			try
			{
				var userId = GetUserIdFromClaims();
				var deleted = await _mailTemplateService.DeleteTemplateAsync(id, userId);

				if (!deleted)
				{
					return NotFound();
				}

				_logger.LogInformation("Deleted template {TemplateId}", id);
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting template {TemplateId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}
	}
}
