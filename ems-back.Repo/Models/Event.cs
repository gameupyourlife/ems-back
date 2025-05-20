using ems_back.Repo.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ems_back.Repo.Models.Types;

[Table("Events")]
public class Event
{
    [Key]
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    public string Title { get; set; }

    [Column(TypeName = "text")]
    public string? Description { get; set; }

    [Required]
    [MaxLength(255)]
    public string Location { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid CreatedBy { get; set; }

    [MaxLength(255)]
    public string? Image { get; set; }

    [Required]
    public string Category { get; set; }

    [Required]
    public EventStatus Status { get; set; }

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid UpdatedBy { get; set; }

    [Required]
    public DateTime Start { get; set; }

    public DateTime? End { get; set; }

    [Required]
    public Guid OrganizationId { get; set; }

    [Required]
    public int AttendeeCount { get; set; }

    [Required]
    public int Capacity { get; set; }

    // for navigation:

    [ForeignKey("CreatedBy")]
    public virtual User Creator { get; set; }

    [ForeignKey("UpdatedBy")]
    public virtual User? Updater { get; set; }

    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }

    public virtual ICollection<EventAttendee> Attendees { get; set; } = new List<EventAttendee>();
    public virtual ICollection<EventFile> Files { get; set; } = new List<EventFile>();
    public virtual ICollection<Mail> Mails { get; set; } = new List<Mail>();
    public virtual ICollection<AgendaEntry> AgendaEntries { get; set; } = new List<AgendaEntry>();
    public virtual ICollection<Flow> Flows { get; set; } = new List<Flow>();
    public virtual ICollection<EventOrganizer> Organizers { get; set; } = new List<EventOrganizer>();
}