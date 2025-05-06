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
		public Guid FlowId { get; set; } = Guid.NewGuid();

		[Required]
		[MaxLength(100)]
		public string Name { get; set; }

		public string? Description { get; set; }

		[Required]
		public bool IsActive { get; set; } = true;

		[Required]
		public bool stillPending { get; set; } = false;

		[Required]
		public bool multipleRuns { get; set; } = false;

		[Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[Required]
		public Guid CreatedBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

		public Guid? UpdatedBy { get; set; }

		[Required]
		public Guid EventId { get; set; }

		public Guid? FlowTemplateId { get; set; }

        [ForeignKey("FlowTemplateId")]
        public virtual FlowTemplate? FlowTemplate { get; set; }

        [ForeignKey("CreatedBy")]
		public virtual User Creator { get; set; }

		[ForeignKey("UpdatedBy")]
		public virtual User? Updater { get; set; }

		[ForeignKey("EventId")]
		public virtual Event Event { get; set; }
	}
}