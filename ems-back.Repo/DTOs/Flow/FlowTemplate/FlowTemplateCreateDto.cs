﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Flow.FlowTemplate
{
	public class FlowTemplateCreateDto
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public Guid OrganizationId { get; set; }
	}
}
