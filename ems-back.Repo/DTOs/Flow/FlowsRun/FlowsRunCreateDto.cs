using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Flow.FlowsRun
{
	public class FlowsRunCreateDto
	{
		public Guid FlowId { get; set; }
		public string Logs { get; set; }
	}
}
