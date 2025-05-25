using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Email.MailRun
{
	public class MailRunDto
	{
		public Guid MailRunId { get; set; }

		[Required]
		public Guid MailId { get; set; }

		[Required]
		public MailRunStatus Status { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }

		public string Logs { get; set; }
	}
}
