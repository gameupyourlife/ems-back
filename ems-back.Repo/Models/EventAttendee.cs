using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Models
{
	[Table("EventAttendees")]
	public class EventAttendee
	{
		[Key, Column(Order = 0)]
		public required Guid EventId { get; set; }

		[Key, Column(Order = 1)]
		public required Guid UserId { get; set; }

        public required DateTime RegisteredAt { get; set; }
		public required AttendeeStatus Status { get; set; }

        // for navigation:

        [ForeignKey("EventId")]
		public virtual Event? Event { get; set; }

		[ForeignKey("UserId")]
		public virtual User? User { get; set; }
    }
}
