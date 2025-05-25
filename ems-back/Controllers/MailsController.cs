using ems_back.Repo.DTOs.Email.MailRun;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Authorization;
using ems_back.Repo.Exceptions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ems_back.Repo.Services;
using ems_back.Repo.DTOs.Mail;

namespace ems_back.Controllers
{
	[ApiController]
	[Route("api/org/{orgId}/events/{eventId}/mails")]
	public class MailsController : ControllerBase
	{
		private readonly IMailService _mailService;
		private readonly ILogger<MailsController> _logger;

		public MailsController(
			IMailService mailService,
			IMailRunService mailRunService,
			ILogger<MailsController> logger)
		{
			_mailService = mailService;
			_logger = logger;
			_logger.LogInformation("MailsController initialized");
		}

        // GET: api/org/{orgId}/events/{eventId}/mails
        [HttpGet]
        [Authorize(Roles = 
			$"{nameof(UserRole.Admin)}, " +
			$"{nameof(UserRole.Owner)}, " +
			$"{nameof(UserRole.Organizer)}, " +
			$"{nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<IEnumerable<MailDto>>> GetMailsForEvent(Guid orgId, Guid eventId)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            if (!await _mailService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
			{
				var mails = await _mailService.GetMailsForEventAsync(orgId, eventId, Guid.Parse(userId));
				return Ok(mails);
			}
            catch (NotFoundException ex)
			{
				return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mails for event {EventId}", eventId);
                return StatusCode(500, "Internal server error");
            }
        }

		// GET: api/org/{orgId}/events/{eventId}/mails/{mailId}
		[HttpGet("{mailId}")]
		[Authorize(Roles =
			$"{nameof(UserRole.Admin)}, " +
			$"{nameof(UserRole.Owner)}, " +
			$"{nameof(UserRole.Organizer)}, " +
			$"{nameof(UserRole.EventOrganizer)}")]

		public async Task<ActionResult<MailDto>> GetMail(Guid orgId, Guid eventId, Guid mailId)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            if (!await _mailService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
			{
				var mail = await _mailService.GetMailByIdAsync(orgId, eventId, mailId, Guid.Parse(userId));
				if (mail == null) return NotFound();
				return Ok(mail);
			}
			catch (MismatchException ex)
			{
				return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving mail {MailId} for event {EventId}", mailId, eventId);
				return StatusCode(500, "Internal server error");
			}
		}

		// POST: api/org/{orgId}/events/{eventId}/mails
		[HttpPost]
		[Authorize(Roles =
			$"{nameof(UserRole.Admin)}, " +
			$"{nameof(UserRole.Owner)}, " +
			$"{nameof(UserRole.Organizer)}, " +
			$"{nameof(UserRole.EventOrganizer)}")]
		public async Task<ActionResult<CreateMailDto>> CreateMail(
			Guid orgId,
			Guid eventId,
			CreateMailDto createMailDto)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            if (!await _mailService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
			{
				var result = await _mailService.CreateMailAsync(orgId, eventId, createMailDto, Guid.Parse(userId));
				if (result == null)
				{
                    _logger.LogWarning("Mail creation failed for event {EventId}", eventId);
                    return BadRequest("Mail creation failed");
                }
				return Ok(result);

            }
			catch (DbUpdateException ex)
			{
				return BadRequest(ex.Message);
            }
			catch (MismatchException ex)
			{
				return BadRequest(ex.Message);
            }
			catch (NotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating mail for event {EventId}", eventId);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPut("{mailId}")]
        [Authorize(Roles =
            $"{nameof(UserRole.Admin)}, " +
            $"{nameof(UserRole.Owner)}, " +
            $"{nameof(UserRole.Organizer)}, " +
            $"{nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<MailDto>> UpdateMail(
			[FromRoute] Guid orgId,
			[FromRoute] Guid eventId,
			[FromRoute] Guid mailId,
			[FromBody] CreateMailDto updateMailDto)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            if (!await _mailService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
			{
				var mail = await _mailService.UpdateMailAsync(orgId, eventId, mailId, updateMailDto, Guid.Parse(userId));
				if (mail == null)
				{
					throw new NotFoundException("Mail not found");
                }
                return Ok(mail);
            }
			catch (MismatchException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (NotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating mail {MailId} for event {EventId}", mailId, eventId);
				return StatusCode(500, "Internal server error");
			}
		}

		// DELETE: api/org/{orgId}/events/{eventId}/mails/{mailId}
		[HttpDelete("{mailId}")]
        [Authorize(Roles =
            $"{nameof(UserRole.Admin)}, " +
            $"{nameof(UserRole.Owner)}, " +
            $"{nameof(UserRole.Organizer)}, " +
            $"{nameof(UserRole.EventOrganizer)}")]
        public async Task<IActionResult> DeleteMail(Guid orgId, Guid eventId, Guid mailId)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            if (!await _mailService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
			{
				var success = await _mailService.DeleteMailAsync(orgId, eventId, mailId, Guid.Parse(userId));
				if (!success) return NotFound();
				return Ok(success);
			}
			catch (MismatchException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting mail {MailId} for event {EventId}", mailId, eventId);
				return StatusCode(500, "Internal server error");
			}
		}

        // POST: api/org/{orgId}/events/{eventId}/mails/{mailId}/send
        [HttpPost("{mailId}/send")]
        [Authorize(Roles = 
			$"{nameof(UserRole.Admin)}, " +
            $"{nameof(UserRole.Owner)}, " +
            $"{nameof(UserRole.Organizer)}, " +
            $"{nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<bool>> SendMail(Guid orgId, Guid eventId, Guid mailId)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            if (!await _mailService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
            {
                await _mailService.SendMailByIdAsync(orgId, eventId, mailId, Guid.Parse(userId));
				
                return Ok(true);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending mail {MailId} for event {EventId}", mailId, eventId);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/org/{orgId}/events/{eventId}/mails/send
        [HttpPost("send")]
        [Authorize(Roles = 
			$"{nameof(UserRole.Admin)}, " +
			$"{nameof(UserRole.Owner)}, " +
			$"{nameof(UserRole.Organizer)}, " +
			$"{nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<bool>> SendMailManual(
			Guid orgId, 
			Guid eventId, 
			[FromBody] CreateMailDto sendMailManualDto)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            if (!await _mailService.ExistsOrg(orgId))
            {
                _logger.LogWarning("Organization with id {OrgId} does not exist", orgId);
                return BadRequest("Organization does not exist");
            }

            try
            {
                await _mailService.SendMailWithDtoAsync(orgId, eventId, sendMailManualDto, Guid.Parse(userId));
                return Ok(true);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending mail manually for event {EventId}", eventId);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/org/{orgId}/events/{eventId}mails/send/manual
        [HttpPost("send/manual")]
        [Authorize(Roles =
            $"{nameof(UserRole.Admin)}, " +
            $"{nameof(UserRole.Owner)}, " +
            $"{nameof(UserRole.Organizer)}, " +
            $"{nameof(UserRole.EventOrganizer)}")]
        public async Task<ActionResult<bool>> SendMailManual(
            Guid orgId,
            [FromBody] MailManualDto manualDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return BadRequest("User ID not found");
            }

            try
            {
                await _mailService.SendMailManualAsync(orgId, manualDto, Guid.Parse(userId));
                return Ok(true);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending mail manually for organization {OrgId}", orgId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
