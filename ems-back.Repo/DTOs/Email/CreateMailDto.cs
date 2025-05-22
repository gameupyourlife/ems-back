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
		public required string Subject { get; set; }

		[Required]
		public required string Body { get; set; }

		[Required]
		public required IEnumerable<Guid> Recipients { get; set; }

		public DateTime? ScheduledFor { get; set; }

		[Required]
		public required Guid EventId { get; set; }
	}
}
