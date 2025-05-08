using ems_back.Repo.Models.Types;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class MailRun

{
    [Key]

    public Guid MainRunId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public Guid MailId { get; set; }

    [Required]
    public MailRunStatus Status { get; set; }

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "jsonb")]
    public string Logs { get; set; }

    // for navigation:
    [ForeignKey("MailId")]
    public virtual Mail Mail { get; set; }
}
