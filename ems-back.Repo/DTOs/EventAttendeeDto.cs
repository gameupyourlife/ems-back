using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// For attendee management
	public class EventAttendeeDto
	{
		public Guid UserId { get; set; }
		public string UserName { get; set; }
		public string ProfilePicture { get; set; }
		public DateTime RegisteredAt { get; set; }
		public bool Attended { get; set; }
	}
}
