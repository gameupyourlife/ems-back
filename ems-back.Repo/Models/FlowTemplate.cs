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
        public Guid FlowTemplateId { get; set; } = new Guid();

        [Required]
		[MaxLength(255)]
		public string Name { get; set; }

		public string Description { get; set; }

		[Required]
		public Guid OrganizationId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }
		public Guid? UpdatedBy { get; set; }

        // for navigation:

        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User Creator { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User? Updater { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual ICollection<Action> Actions { get; set; } = new List<Action>();
        public virtual ICollection<Trigger> Triggers { get; set; } = new List<Trigger>();
        public virtual ICollection<Flow> Flows { get; set; } = new List<Flow>();
    }
}
