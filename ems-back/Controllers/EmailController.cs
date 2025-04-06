using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class MailController : ControllerBase
{
    private readonly MailService _mailService;

    public MailController(MailService mailService)
    {
        _mailService = mailService;
    }

    [HttpPost("send")]
    public IActionResult SendMail([FromQuery] string to)
    {
        _mailService.SendEmail(to, "Testmail", "Das ist eine Testmail von deiner API 😎");
        return Ok("Mail gesendet!");
    }
}

/*using ems_back.Emails;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ems_back.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("EmailController");

            var receiver = "rebisanik@web.de";
            var subject = "Test Email";
            var message = "This is a test email from the EMS application.";

            await _emailSender.SendEmailAsync(receiver, subject, message);

            return Ok("Email sent.");
        }
    }
}*/
