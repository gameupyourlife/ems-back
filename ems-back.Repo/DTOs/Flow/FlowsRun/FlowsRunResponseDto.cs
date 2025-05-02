using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Flow.FlowsRun
{
	public class FlowsRunResponseDto
	{
		public int Id { get; set; }
		public Guid FlowId { get; set; }
		public string Status { get; set; }
		public DateTime Timestamp { get; set; }
		public string Logs { get; set; }
	}
}
