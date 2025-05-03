using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models.Types
{
	public enum FlowRunStatus
	{
		Pending,
		Running,
		Completed,
		Failed,
		Cancelled
	}
}
