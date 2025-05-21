using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Trigger;

namespace ems_back.Repo.DTOs.Flow.FlowTemplate
{
	public class FlowTemplateResponseDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public Guid OrganizationId { get; set; }
		public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public List<TriggerOverviewDto> Triggers { get; set; }
        public List<ActionOverviewDto> Actions { get; set; }
    }
}
