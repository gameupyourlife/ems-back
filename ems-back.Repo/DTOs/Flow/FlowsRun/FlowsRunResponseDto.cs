using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.DTOs.Flow.FlowsRun
{
	public class FlowsRunResponseDto
	{
		public Guid FlowId { get; set; }
		public Guid EventId { get; set; }
        public required FlowRunStatus Status { get; set; }
		public DateTime? Timestamp { get; set; }
		public string? Logs { get; set; }
	}
}
