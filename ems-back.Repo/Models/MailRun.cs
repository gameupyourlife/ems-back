using ems_back.Repo.Models.Types;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class MailRun

{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public Guid MainRunId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]

    public string MailId { get; set; }

    [Required]
    public MailRunStatus Status { get; set; }

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "jsonb")]
    public string Logs { get; set; }
}
