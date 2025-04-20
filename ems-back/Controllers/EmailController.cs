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

    // POST: /api/org/{orgId}/mails
    [HttpPost("emails")]
    public async Task<ActionResult<PlaceholderDTO>> CreateMailTemplate(Guid orgId, [FromBody] PlaceholderDTO bodyDto)
    {
        throw new NotImplementedException();
    }

    // POST: /api/org/{orgId}/events/{eventId}/email
    [HttpPost("events/{eventId}/email")]
    public async Task<ActionResult<PlaceholderDTO>> SendEventMail(Guid orgId, Guid eventId,  [FromBody] PlaceholderDTO bodyDto)
    {
        throw new NotImplementedException();
    }
}
