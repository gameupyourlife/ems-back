using ems_back.Repo.DTOs.Email.MailRun;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/events/{eventId}/[controller]")]
public class MailsController : ControllerBase
{
	private readonly IMailService _mailService;

	public MailsController(IMailService mailService)
	{
		_mailService = mailService;
	}

	// GET: api/events/{eventId}/mails
	[HttpGet]
	public async Task<ActionResult<IEnumerable<EmailDto>>> GetMailsForEvent(Guid eventId)
	{
		var mails = await _mailService.GetMailsForEventAsync(eventId);
		return Ok(mails);
	}

	// GET: api/events/{eventId}/mails/{id}
	[HttpGet("{id}")]
	public async Task<ActionResult<EmailDto>> GetMail(Guid eventId, Guid id)
	{
		var mail = await _mailService.GetMailForEventAsync(eventId, id);
		if (mail == null)
		{
			return NotFound();
		}
		return Ok(mail);
	}

	// POST: api/events/{eventId}/mails
	[HttpPost]
	public async Task<ActionResult<EmailDto>> CreateMail(Guid eventId, CreateMailDto createMailDto)
	{
		// Ensure the mail is created for the correct event
		createMailDto.EventId = eventId;

		var createdMail = await _mailService.CreateMailAsync(createMailDto);
		return CreatedAtAction(
			nameof(GetMail),
			new { eventId, id = createdMail.MailId },
			createdMail
		);
	}

	// PUT: api/events/{eventId}/mails/{id}
	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateMail(Guid eventId, Guid id, UpdateMailDto updateMailDto)
	{
		var result = await _mailService.UpdateMailForEventAsync(eventId, id, updateMailDto);
		if (!result)
		{
			return NotFound();
		}
		return NoContent();
	}

	// DELETE: api/events/{eventId}/mails/{id}
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteMail(Guid eventId, Guid id)
	{
		var result = await _mailService.DeleteMailForEventAsync(eventId, id);
		if (!result)
		{
			return NotFound();
		}
		return NoContent();
	}

	// GET: api/events/{eventId}/mails/{id}/runs
	[HttpGet("{id}/runs")]
	public async Task<ActionResult<IEnumerable<MailRunDto>>> GetMailRunsForMail(Guid eventId, Guid id)
	{
		var runs = await _mailService.GetMailRunsForMailAsync(eventId, id);
		return Ok(runs);
	}
}