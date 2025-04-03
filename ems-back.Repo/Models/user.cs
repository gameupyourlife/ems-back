using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ems_back.Repo.Models
{
	public class User : IdentityUser<Guid>
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public override Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[Required]
		[MaxLength(100)]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(100)]
		public string LastName { get; set; }

		[Required]
		public UserRole Role { get; set; } = UserRole.Participant;

		public bool IsEmailConfirmed { get; set; } = false;

		[MaxLength(100)]
		public string? EmailConfirmationToken { get; set; }

		[MaxLength(255)]
		public string? ProfilePicture { get; set; }

		// Navigation properties
		public virtual ICollection<Organization> CreatedOrganizations { get; set; } = new List<Organization>();
		public virtual ICollection<Event> CreatedEvents { get; set; } = new List<Event>();
		public virtual ICollection<EventAttendee> AttendedEvents { get; set; } = new List<EventAttendee>();

		public virtual ICollection<OrganizationUser> OrganizationUsers { get; set; } = new HashSet<OrganizationUser>();
		[NotMapped]
		public string FullName => $"{FirstName} {LastName}";
	}
}