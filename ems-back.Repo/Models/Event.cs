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
    public required string Title { get; set; }

    [Column(TypeName = "text")]
    public string? Description { get; set; }

    [Required]
    [MaxLength(255)]
    public required string Location { get; set; }

    [Required]
    public required DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    [MaxLength(255)]
    public string? Image { get; set; }

    [Required]
    public required string Category { get; set; }

    [Required]
    public required EventStatus Status { get; set; }

    [Required]
    public required DateTime UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    [Required]
    public required DateTime Start { get; set; }

    public DateTime? End { get; set; }

    [Required]
    public required Guid OrganizationId { get; set; }

    [Required]
    public required int AttendeeCount { get; set; }

    [Required]
    public required int Capacity { get; set; }

    // for navigation:

    [ForeignKey("CreatedBy")]
    public virtual User? Creator { get; set; }

    [ForeignKey("UpdatedBy")]
    public virtual User? Updater { get; set; }

    [ForeignKey("OrganizationId")]
    public virtual Organization? Organization { get; set; }
    public virtual ICollection<EventAttendee>? Attendees { get; set; }
    public virtual ICollection<Mail>? Mails { get; set; }
    public virtual ICollection<AgendaEntry>? AgendaEntries { get; set; }
    public virtual ICollection<Flow>? Flows { get; set; }
    public virtual ICollection<EventOrganizer>? Organizers { get; set; }
}