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
		public Guid FlowId { get; set; } = Guid.NewGuid();

		[Required]
		[MaxLength(100)]
		public required string Name { get; set; }

		public string? Description { get; set; }

		[Required]
		public required bool IsActive { get; set; } = true;

		[Required]
		public required bool stillPending { get; set; } = false;

		[Required]
		public required bool multipleRuns { get; set; }

		[Required]
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public Guid? CreatedBy { get; set; }

        public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		public Guid? UpdatedBy { get; set; }

		[Required]
		public required Guid EventId { get; set; }

		public Guid? FlowTemplateId { get; set; }

        // for navigation:

        [ForeignKey("CreatedBy")]
		public virtual User? Creator { get; set; }

		[ForeignKey("UpdatedBy")]
		public virtual User? Updater { get; set; }

		[ForeignKey("EventId")]
		public virtual Event? Event { get; set; }

		[ForeignKey("FlowTemplateId")]
        public virtual FlowTemplate? FlowTemplate { get; set; }
        public virtual ICollection<FlowsRun> FlowsRuns { get; set; } = new List<FlowsRun>();
        public virtual ICollection<Action> Actions { get; set; } = new List<Action>();
        public virtual ICollection<Trigger> Triggers { get; set; } = new List<Trigger>();
    }
}