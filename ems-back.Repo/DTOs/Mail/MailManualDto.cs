using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Mail
{
    public class MailManualDto
    {
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public required IEnumerable<string> recipentMails { get; set; }
    }
}
