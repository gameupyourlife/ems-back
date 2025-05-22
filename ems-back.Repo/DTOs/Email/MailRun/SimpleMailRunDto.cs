using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Email.MailRun
{
	public class SimpleMailRunDto
	{
		//Used in UI lists, logs, monitoring dashboards — no heavy fields like JSON logs.

		public Guid MailRunId { get; set; }
		public Guid MailId { get; set; }
		public MailRunStatus Status { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
