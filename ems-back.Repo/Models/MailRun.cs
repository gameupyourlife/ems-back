using ems_back.Repo.Models.Types;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ems_back.Repo.Models
{
    public class MailRun
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required Guid MailId { get; set; }

        [Required]
        public required MailRunStatus Status { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string? Logs { get; set; }

        // for navigation:

        [ForeignKey("MailId")]
        public virtual Mail? Mail { get; set; }
    }
}