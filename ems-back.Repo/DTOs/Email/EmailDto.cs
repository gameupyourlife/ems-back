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
        public string Id { get; set; } //  Evtl. in Guid umwandeln
        public Guid EventId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
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
