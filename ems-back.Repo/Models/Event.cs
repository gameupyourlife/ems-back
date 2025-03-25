using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models
{
	public class Event
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[MaxLength(200)]
		public string Name { get; set; }

		[Required]
		public DateTime StartDate { get; set; }

		[Required]
		public DateTime EndDate { get; set; }

		[MaxLength(500)]
		public string Description { get; set; }

		[Required]
		public int OrganizationId { get; set; }

		[Required]
		public int MaxParticipants { get; set; }  // Capacity management

		[ForeignKey("OrganizationId")]
		public Organization Organization { get; set; }

		public ICollection<User> Participants { get; set; } = new List<User>();
		[Required]
		public EventStatus Status { get; set; } = EventStatus.Upcoming; // Default: Upcoming
		public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
	} 
}
