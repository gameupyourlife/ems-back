using ems_back.Repo.DTOs.Email.MailRun;
using ems_back.Repo.Services;
using Microsoft.AspNetCore.Mvc;

namespace ems_back.Controllers
{
	[ApiController]
	[Route("api/events/{eventId}/mails/{mailId}/[controller]")]
	public class MailRunsController : ControllerBase
	{
		private readonly IMailRunService _mailRunService;

		public MailRunsController(IMailRunService mailRunService)
		{
			_mailRunService = mailRunService;
		}

		// GET: api/events/{eventId}/mails/{mailId}/mailruns
		[HttpGet]
		public async Task<ActionResult<IEnumerable<MailRunDto>>> GetMailRuns(Guid eventId, Guid mailId)
		{
			var runs = await _mailRunService.GetMailRunsForMailAsync(eventId, mailId);
			return Ok(runs);
		}

		// GET: api/events/{eventId}/mails/{mailId}/mailruns/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<MailRunDto>> GetMailRun(Guid eventId, Guid mailId, Guid id)
		{
			var run = await _mailRunService.GetMailRunForMailAsync(eventId, mailId, id);
			if (run == null)
			{
				return NotFound();
			}
			return Ok(run);
		}

		// POST: api/events/{eventId}/mails/{mailId}/mailruns
		[HttpPost]
		public async Task<ActionResult<MailRunDto>> CreateMailRun(Guid eventId, Guid mailId, CreateMailRunDto createMailRunDto)
		{
			// Ensure the mail run is created for the correct mail
			createMailRunDto.MailId = mailId;

			var createdRun = await _mailRunService.CreateMailRunAsync(createMailRunDto);
			return CreatedAtAction(
				nameof(GetMailRun),
				new { eventId, mailId, id = createdRun.MailRunId },
				createdRun
			);
		}

		// PATCH: api/events/{eventId}/mails/{mailId}/mailruns/{id}/status
		//[HttpPatch("{id}/status")]
		//public async Task<IActionResult> UpdateMailRunStatus(Guid eventId, Guid mailId, Guid id, UpdateMailRunDto statusDto)
		//{
		//	var result = await _mailRunService.UpdateMailRunStatusAsync(eventId, mailId, id, UpdateMailRunDto.Status);
		//	if (!result)
		//	{
		//		return NotFound();
		//	}
		//	return NoContent();
		//}
	}
}
