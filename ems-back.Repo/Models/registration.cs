using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models
{
	public class Registration
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required]
		public int OrganizationId { get; set; }

		[Required]
		public int EventId { get; set; }

		[Required]
		public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

		// Navigation properties
		[ForeignKey("UserId")]
		public User User { get; set; }

		[ForeignKey("OrganizationId")]
		public Organization Organization { get; set; }

		[ForeignKey("EventId")]
		public Event Event { get; set; }
	}
}
