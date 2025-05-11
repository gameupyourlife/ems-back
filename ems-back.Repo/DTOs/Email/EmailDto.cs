using ems_back.Repo.DTOs.User;
using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Email
{
    public class EmailDto
    {
        public Guid MailId { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsUserCreated { get; set; }
        public List<Guid> Recipients { get; set; }
        public MailRunStatus Status { get; set; }
        public DateTime? Scheduledfor { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public Guid? FlowId { get; set; }
    }
}
