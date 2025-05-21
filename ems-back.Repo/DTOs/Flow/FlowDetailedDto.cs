using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Trigger;

namespace ems_back.Repo.DTOs.Flow
{
    // Full detailed response (with triggers and actions)
    public class FlowDetailedDto : FlowResponseDto
    {
	    public bool StillPending { get; set; } = false;
	    public bool MultipleRuns { get; set; } = false;
	    public Guid OrganizationId { get; set; }
	    public Guid EventId { get; set; } // <-- Important!
		public List<TriggerDto> Triggers { get; set; } = new();
        public List<ActionDto> Actions { get; set; } = new();
    }
}
