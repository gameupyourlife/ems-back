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
	public class FlowsRun
	{
		[Key]
        public Guid Id { get; set; } = new Guid();

        [Required]
		public required Guid FlowId { get; set; }

		[Required]
		[MaxLength(50)]
		public required FlowRunStatus Status { get; set; }

		public DateTime? Timestamp { get; set; }

		public string? Logs { get; set; }

        // for navigation:
        [ForeignKey("FlowId")]
        public Flow? Flow { get; set; }
    }
}