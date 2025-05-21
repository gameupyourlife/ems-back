using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Email.MailRun
{
	public class CreateMailRunDto
	{
		[Required]
		public Guid MailId { get; set; }

		[Required]
		public MailRunStatus Status { get; set; }

		public string Logs { get; set; }
	}
}
