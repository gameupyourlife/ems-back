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
		public required string Name { get; set; }
		public required string Description { get; set; }
        public required Guid CreatedBy { get; set; }
	}
}
