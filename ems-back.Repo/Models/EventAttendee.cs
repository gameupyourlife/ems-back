using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models
{
	[Table("EventAttendees")]
	public class EventAttendee
	{
		[Key, Column(Order = 0)]
		public Guid EventId { get; set; }

		[Key, Column(Order = 1)]
		public Guid UserId { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
		public bool Attended { get; set; } = false;

        // for navigation:

        [ForeignKey("EventId")]
		public virtual Event Event { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
    }
}
