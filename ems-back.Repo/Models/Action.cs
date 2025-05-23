using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Models
{

	[Table("Actions")]
	public class Action
	{
		[Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
		[MaxLength(100)]
		public required ActionType Type { get; set; }

        [Required]
        [Column(TypeName = "jsonb")]
		public required string Details { get; set; }

        // enthält mailtemplate id und bool sendToNewAttendee

		[Required]
        public required DateTime CreatedAt { get; set; }

        public Guid? FlowId { get; set; }

        public Guid? FlowTemplateId { get; set; }

        // for navigation:

        [ForeignKey("FlowId")]
		public virtual Flow? Flow { get; set; }

		[ForeignKey("FlowTemplateId")]
		public virtual FlowTemplate? FlowTemplate { get; set; }
    }
}
