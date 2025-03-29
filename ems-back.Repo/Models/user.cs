using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ems_back.Repo.Models
{

	public class User : IdentityUser<Guid>
	{
		

		[Required]
		[MaxLength(100)]
		public string? FirstName { get; set; }

		[Required]
		[MaxLength(100)]
		public string? LastName { get; set; }

		// Changed to Guid to match UML
		public Guid OrganizationId { get; set; }

		[Required]
		public UserRole Role { get; set; } = UserRole.Participant;

		public bool IsEmailConfirmed { get; set; } = false;

		[MaxLength(100)]  
		public string? EmailConfirmationToken { get; set; }

	
		[MaxLength(255)]
		public string? ProfilePicture { get; set; }
		[ForeignKey("OrganizationId")]
		public virtual Organization? Organization { get; set; }

		// From UML relationships
		public virtual ICollection<Organization>? CreatedOrganizations { get; set; }
		public virtual ICollection<Event>? CreatedEvents { get; set; }
		public virtual ICollection<EventAttendee>? AttendedEvents { get; set; }

		// Computed property for full name
		[NotMapped]
		public string? FullName => $"{FirstName} {LastName}";
	}

}
