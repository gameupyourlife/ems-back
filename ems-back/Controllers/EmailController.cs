using ems_back.Repo.DTOs.Email;
using ems_back.Repo.DTOs.Placeholder;
using ems_back.Repo.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/org/{orgId}")]
public class MailController : ControllerBase
{
    private readonly IEmailService _mailService;
    private readonly ILogger<MailController> _logger;

    public MailController(IEmailService mailService, ILogger<MailController> logger)
    {
        _mailService = mailService;
        _logger = logger;
    }

    // GET: api/org/{orgId}/emails
    [HttpGet("emails")]
    public async Task<ActionResult<IEnumerable<EmailDto>>> GetMailTemplates(Guid orgId)
    {
        throw new NotImplementedException();
    }

    // POST: /api/org/{orgId}/emails
    [HttpPost("emails")]
    public async Task<ActionResult<PlaceholderDTO>> CreateMailTemplate(
        [FromRoute] Guid orgId, 
        [FromRoute] PlaceholderDTO bodyDto)
    {
        throw new NotImplementedException();
    }

    // GET: /api/org/{orgId}/events/{eventId}/emails
    [HttpGet("events/{eventId}/emails")]
    public async Task<ActionResult<IEnumerable<EmailDto>>> GetEventMails(
        [FromRoute] Guid orgId, 
        [FromRoute] Guid eventId)
    {
        var emails = await _mailService.GetEventMails(orgId, eventId);
        if (emails == null)
        {
            _logger.LogWarning("No emails found for organization with id {OrgId} and event with id {EventId}", orgId, eventId);
            return NotFound();
        }
        return Ok(emails);
    }

    // POST: /api/org/{orgId}/events/{eventId}/emails
    [HttpPost("events/{eventId}/emails")]
    public async Task<ActionResult<PlaceholderDTO>> SendEventMail(
        [FromRoute] Guid orgId, 
        [FromRoute] Guid eventId,  
        [FromBody] PlaceholderDTO bodyDto)
    {
        throw new NotImplementedException();
    }
}
