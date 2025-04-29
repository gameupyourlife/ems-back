using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models
{
	public class FlowTemplate
	{
		[Key]
		public Guid FlowTemplateId { get; set; }

		[Required]
		[MaxLength(255)]
		public string Name { get; set; }

		public string Description { get; set; }

		[Required]
		public Guid OrganizationId { get; set; }

		[ForeignKey("OrganizationId")]
		public Organization Organization { get; set; }

		public DateTime CreatedAt { get; set; }
		public Guid CreatedBy { get; set; }
		public DateTime UpdatedAt { get; set; }
		public Guid UpdatedBy { get; set; }
	}
}
