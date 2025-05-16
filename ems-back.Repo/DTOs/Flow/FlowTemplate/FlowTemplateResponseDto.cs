using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Flow.FlowTemplate
{
	public class FlowTemplateResponseDto
	{
		public Guid FlowTemplateId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public Guid OrganizationId { get; set; }
		public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
