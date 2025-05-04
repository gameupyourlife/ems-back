using ems_back.Repo.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ems_back.Repo.Models.Types;

[Table("Events")]
public class Event
{
	[Key]

	[Required]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid OrganizationId { get; set; }

    [Required]
	[MaxLength(255)]
	public string Title { get; set; }

	public DateTime? Date { get; set; }

	[Column(TypeName = "text")]
	public string? Description { get; set; }

	[MaxLength(255)]
	public string Location { get; set; }

	[Required]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	[Required]
	public Guid CreatedBy { get; set; }

	[MaxLength(255)]
	public string? Image { get; set; }

	public EventCategory Category { get; set; }

	public EventStatus Status { get; set; }

	[Required]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

	[Required]
	public Guid UpdatedBy { get; set; }

	public DateTime Start { get; set; }
	public DateTime End { get; set; }

	// Navigation properties
	[ForeignKey("CreatedBy")]
	public virtual User? Creator { get; set; }

	[ForeignKey("UpdatedBy")]
	public virtual User Updater { get; set; }

    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }

    public virtual ICollection<EventAttendee> Attendees { get; set; }
	public virtual ICollection<AgendaEntry> AgendaItems { get; set; }
}