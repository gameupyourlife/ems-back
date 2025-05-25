using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models
{
    [Table("MailQueueEntries")]
    public class MailQueueEntry
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required string ToEmail { get; set; }

        [Required]
        public required string ToName { get; set; }

        [Required]
        public required string Subject { get; set; }

        [Required]
        public required string Body { get; set; }

        public MailRunStatus Status { get; set; } = MailRunStatus.Pending;
        public int RetryCount { get; set; } = 0;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
