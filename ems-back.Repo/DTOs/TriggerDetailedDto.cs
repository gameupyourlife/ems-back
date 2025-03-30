using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// Detailed response DTO (with Flow information)
	public class TriggerDetailedDto : TriggerDto
	{
		public FlowBasicDto Flow { get; set; }
	}
}
