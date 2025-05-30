﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ems_back.Repo.Models
{
    public class Mail
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Subject { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public required string Body { get; set; }

        public string? Description { get; set; }

        [Required]
        public required bool IsUserCreated { get; set; }

        public IEnumerable<Guid>? Recipients { get; set; }

        [Required]
        public required bool sendToAllParticipants { get; set; }

        public DateTime? ScheduledFor { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        [Required]
        public Guid EventId { get; set; }

        // for navigation:

        [ForeignKey("EventId")]
        public virtual Event? Event { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User? Creator { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User? Updater { get; set; }
        public virtual ICollection<MailRun> MailRuns { get; set; } = new List<MailRun>();
    }
}