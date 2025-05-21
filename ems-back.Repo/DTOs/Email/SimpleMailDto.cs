using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Email
{
	public class SimpleMailDto
	{
		public Guid MailId { get; set; }
		public string Name { get; set; }
		public string Subject { get; set; }
		public DateTime? ScheduledFor { get; set; }
		public DateTime CreatedAt { get; set; }
		public Guid EventId { get; set; }
	}
}
