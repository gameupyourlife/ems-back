using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// Full detailed response (with triggers and actions)
	public class FlowDetailedDto : FlowResponseDto
	{
		public List<TriggerDto> Triggers { get; set; } = new();
		public List<ActionDto> Actions { get; set; } = new();
	}
}
