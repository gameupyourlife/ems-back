using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ems_back.Repo.Models
{
	[Table("Flows")]
	public class Flow
	{
		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid FlowId { get; set; } = Guid.NewGuid(); // renamed from Id to FlowId

		[Required]
		[MaxLength(100)]
		public string Name { get; set; }

		public string? Description { get; set; }

		[Required]
		public bool IsActive { get; set; } = true;

		[Required]
		public bool stillPending { get; set; } = false; // NEW

		[Required]
		public bool multipleRuns { get; set; } = false; // NEW

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[Required]
		public Guid CreatedBy { get; set; }

		[Required]
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		[Required]
		public Guid UpdatedBy { get; set; }

		[Required]
		public Guid OrganizationId { get; set; }

		[Required]
		public Guid EventId { get; set; } // NEW (foreign key to Event)

		// Navigation properties
		[ForeignKey("CreatedBy")]
		public virtual User Creator { get; set; }

		[ForeignKey("UpdatedBy")]
		public virtual User Updater { get; set; }

		[ForeignKey("OrganizationId")]
		public virtual Organization Organization { get; set; }

		[ForeignKey("EventId")]
		public virtual Event Event { get; set; } // NEW (navigation)

		public virtual List<Trigger> Triggers { get; set; } = new();
		public virtual List<Action> Actions { get; set; } = new();

		[Required]
		public String FlowType { get; set; } = "Template"; // Default value for Type
	}
}