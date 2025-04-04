﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// Response DTO
	public class AgendaEntryDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		public Guid EventId { get; set; }
		public EventBasicDto Event { get; set; }
	}
}
