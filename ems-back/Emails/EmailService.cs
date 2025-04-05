using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class MailService
{
    private readonly IConfiguration _config;

    public MailService(IConfiguration config)
    {
        _config = config;
    }

    public void SendEmail(string to, string subject, string body)
    {
        var smtpClient = new SmtpClient("smtp.ionos.de")
        {
            Port = 587,
            Credentials = new NetworkCredential(
                _config["Mail:Username"],
                _config["Mail:Password"]
            ),
            EnableSsl = true,
        };

        var mail = new MailMessage
        {
            From = new MailAddress(_config["Mail:Username"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        mail.To.Add(to);

        smtpClient.Send(mail);
    }
}
