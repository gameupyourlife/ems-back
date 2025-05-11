using ems_back.Repo.DTOs.Placeholder;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/org/{orgId}")]
public class MailController : ControllerBase
{
    private readonly MailService _mailService;

    public MailController(MailService mailService)
    {
        _mailService = mailService;
    }

    // GET: api/org/{orgId}/emails
    [HttpGet("emails")]
    public async Task<ActionResult<IEnumerable<PlaceholderDTO>>> GetMailTemplates(Guid orgId)
    {
        throw new NotImplementedException();
    }

    // POST: /api/org/{orgId}/emails
    [HttpPost("emails")]
    public async Task<ActionResult<PlaceholderDTO>> CreateMailTemplate(Guid orgId, [FromBody] PlaceholderDTO bodyDto)
    {
        throw new NotImplementedException();
    }

    // GET: /api/org/{orgId}/events/{eventId}/emails
    [HttpGet("events/{eventId}/emails")]
    public async Task<ActionResult<IEnumerable<PlaceholderDTO>>> GetEventMailTemplates(Guid orgId, Guid eventId)
    {
        throw new NotImplementedException();
    }

    // POST: /api/org/{orgId}/events/{eventId}/emails
    [HttpPost("events/{eventId}/emails")]
    public async Task<ActionResult<PlaceholderDTO>> SendEventMail(Guid orgId, Guid eventId,  [FromBody] PlaceholderDTO bodyDto)
    {
        throw new NotImplementedException();
    }
}
