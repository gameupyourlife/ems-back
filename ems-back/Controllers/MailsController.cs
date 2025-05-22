using ems_back.Repo.DTOs.Email.MailRun;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

		// === MAILS ===

		[HttpGet]
		public async Task<ActionResult<IEnumerable<MailDto>>> GetMailsForEvent(Guid orgId, Guid eventId)
		{
			var mails = await _mailService.GetMailsForEventAsync(orgId, eventId);
			return Ok(mails);
		}

		[HttpGet("{mailId}")]
		public async Task<ActionResult<MailDto>> GetMail(Guid orgId, Guid eventId, Guid mailId)
		{
			var mail = await _mailService.GetMailByIdAsync(orgId, eventId, mailId);
			if (mail == null)
				return NotFound();

			return Ok(mail);
		}

		[HttpPost]
		public async Task<ActionResult<MailDto>> CreateMail(Guid orgId, Guid eventId, CreateMailDto createMailDto)
		{
			var createdMail = await _mailService.CreateMailAsync(orgId, eventId, createMailDto);
			return CreatedAtAction(
				nameof(GetMail),
				new { orgId, eventId, mailId = createdMail.MailId },
				createdMail
			);
		}

		[HttpPut("{mailId}")]
		public async Task<IActionResult> UpdateMail(Guid orgId, Guid eventId, Guid mailId, UpdateMailDto updateMailDto)
		{
			var result = await _mailService.UpdateMailAsync(orgId, eventId, mailId, updateMailDto);
			if (!result)
				return NotFound();

			return NoContent();
		}

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
