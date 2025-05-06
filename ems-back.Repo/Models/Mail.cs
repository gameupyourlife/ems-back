using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Mail
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public Guid MailId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]

    public string Name { get; set; }

    [Required]
    [MaxLength(200)]
    public string Subject { get; set; }

    [Required]
    [Column(TypeName = "text")]
    public string Body { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    [Required]
    public bool IsUserCreated { get; set; } = false;

    [Required]
    public Guid EventId { get; set; }

    [ForeignKey("EventId")]
    public virtual Event Event { get; set; }
}