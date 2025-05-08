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
		[MaxLength(100)]
		public ActionType Type { get; set; }

        [Required]
        [Column(TypeName = "jsonb")]
		public string Details { get; set; }

		[Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid? FlowId { get; set; }

        public Guid? FlowTemplateId { get; set; }

        // for navigation:

        [ForeignKey("FlowId")]
		public virtual Flow Flow { get; set; }

		[ForeignKey("FlowTemplateId")]
		public virtual FlowTemplate FlowTemplate { get; set; }
    }
}
