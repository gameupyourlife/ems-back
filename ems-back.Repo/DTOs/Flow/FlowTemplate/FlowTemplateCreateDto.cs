using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Flow.FlowTemplate
{
	public class FlowTemplateCreateDto
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string? Description { get; set; }
		public Guid OrganizationId { get; set; }
		[Required]
        public Guid CreatedBy { get; set; }
	}
}
