using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Email
{
	public class CreateMailDto
	{
		[Required]
		[MaxLength(200)]
		public string Name { get; set; }

		[Required]
		[MaxLength(200)]
		public string Subject { get; set; }

		[Required]
		public string Body { get; set; }

		[Required]
		public IEnumerable<Guid> Recipients { get; set; }

		public DateTime? ScheduledFor { get; set; }

		[Required]
		public Guid EventId { get; set; }
	}
}
