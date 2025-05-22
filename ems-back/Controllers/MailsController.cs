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

namespace ems_back.Controllers
{
	[ApiController]
	[Route("api/org/{orgId}/events/{eventId}/mails")]
	public class MailsController : ControllerBase
	{
		private readonly IMailService _mailService;
		private readonly IMailRunService _mailRunService;
		private readonly ILogger<MailsController> _logger;

		public MailsController(
			IMailService mailService,
			IMailRunService mailRunService,
			ILogger<MailsController> logger)
		{
			_mailService = mailService;
			_mailRunService = mailRunService;
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
			try
			{
				var mails = await _mailService.GetMailsForEventAsync(orgId, eventId);
				return Ok(mails);
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mails for event {EventId}", eventId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/org/{orgId}/events/{eventId}/mails/{mailId}
        [HttpGet("{mailId}")]
		public async Task<ActionResult<MailDto>> GetMail(Guid orgId, Guid eventId, Guid mailId)
		{
			var mail = await _mailService.GetMailByIdAsync(orgId, eventId, mailId);
			if (mail == null)
				return NotFound();

			return Ok(mail);
		}

        // POST: api/org/{orgId}/events/{eventId}/mails
        [HttpPost]
		public async Task<ActionResult<CreateMailDto>> CreateMail(Guid orgId, Guid eventId, CreateMailDto createMailDto)
		{
			throw new NotImplementedException("CreateMail is not implemented yet.");
        }

		[HttpPut("{mailId}")]
		public async Task<IActionResult> UpdateMail(
			[FromRoute] Guid orgId,
            [FromRoute] Guid eventId, 
			[FromRoute] Guid mailId,
			[FromBody] CreateMailDto updateMailDto)
		{
			throw new NotImplementedException("UpdateMail is not implemented yet.");
		}

        // DELETE: api/org/{orgId}/events/{eventId}/mails/{mailId}
        [HttpDelete("{mailId}")]
		public async Task<IActionResult> DeleteMail(Guid orgId, Guid eventId, Guid mailId)
		{
			var result = await _mailService.DeleteMailAsync(orgId, eventId, mailId);
			if (!result)
				return NotFound();

			return NoContent();
		}

		// === MAIL RUNS ===

		[HttpGet("{mailId}/runs")]
		public async Task<ActionResult<IEnumerable<MailRunDto>>> GetMailRunsForMail(Guid orgId, Guid eventId, Guid mailId)
		{
			var runs = await _mailRunService.GetMailRunsForMailAsync(orgId, eventId, mailId);
			return Ok(runs);
		}

		[HttpGet("{mailId}/runs/{runId}")]
		public async Task<ActionResult<MailRunDto>> GetMailRun(Guid orgId, Guid eventId, Guid mailId, Guid runId)
		{
			var run = await _mailRunService.GetMailRunByIdAsync(orgId, eventId, mailId, runId);
			if (run == null)
				return NotFound();

			return Ok(run);
		}

		[HttpPost("{mailId}/runs")]
		public async Task<ActionResult<MailRunDto>> CreateMailRun(Guid orgId, Guid eventId, Guid mailId, CreateMailRunDto createDto)
		{
			var result = await _mailRunService.CreateMailRunAsync(orgId, eventId, mailId, createDto);
			return CreatedAtAction(
				nameof(GetMailRun),
				new { orgId, eventId, mailId, runId = result.MailRunId },
				result
			);
		}

		[HttpDelete("{mailId}/runs/{runId}")]
		public async Task<IActionResult> DeleteMailRun(Guid orgId, Guid eventId, Guid mailId, Guid runId)
		{
			var success = await _mailRunService.DeleteMailRunAsync(orgId, eventId, mailId, runId);
			if (!success)
				return NotFound();

			return NoContent();
		}
	}
}
