using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models
{

	[Table("Actions")]
	public class Action
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
		[MaxLength(100)]
		public string? Type { get; set; }  // "SendEmail", "CreateTask", etc.

		[Column(TypeName = "jsonb")]
		public string? Details { get; set; }  // JSON configuration

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// Relationships
		public Guid? FlowId { get; set; }

		[ForeignKey("FlowId")]
		public virtual Flow Flow { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
    }

}
