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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FlowTemplateId { get; set; } = new Guid();

        [Required]
		[MaxLength(255)]
		public string Name { get; set; }

		public string Description { get; set; }

		[Required]
		public Guid OrganizationId { get; set; }

		[ForeignKey("OrganizationId")]
		public Organization Organization { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
		public Guid? UpdatedBy { get; set; }
    }
}
