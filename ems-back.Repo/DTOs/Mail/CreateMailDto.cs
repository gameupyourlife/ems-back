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
		[MaxLength(200)]
		public required string Name { get; set; }
		public required string Subject { get; set; }
		public string? Description { get; set; }
		public required string Body { get; set; }
		public IEnumerable<Guid>? Recipients { get; set; }
		public DateTime? ScheduledFor { get; set; }
        public required bool sendToAllParticipants { get; set; }
    }
}