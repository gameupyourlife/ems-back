using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Email
{
	public class UpdateMailDto
	{
		public string? Name { get; set; }

		public string? Subject { get; set; }

		public string? Body { get; set; }

		public IEnumerable<Guid> Recipients { get; set; }

		public DateTime? ScheduledFor { get; set; }
	}
}
