using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// For toggling status
	public class FlowStatusDto
	{
		public bool IsActive { get; set; }
		public Guid UpdatedBy { get; set; }
	}
}
