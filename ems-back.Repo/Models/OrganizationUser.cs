using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ems_back.Repo.Models
{
	[Table("OrganizationUsers")]
	public class OrganizationUser
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; } = Guid.NewGuid();

		// Organization Information
		[Required]
		public Guid OrganizationId { get; set; }

		[Required]
		[MaxLength(255)]
		public string OrganizationName { get; set; }

		[MaxLength(500)]
		public string OrganizationAddress { get; set; }

		[Column(TypeName = "text")]
		public string? OrganizationDescription { get; set; }

		[MaxLength(255)]
		public string? OrganizationProfilePicture { get; set; }

		[MaxLength(255)]
		public string? OrganizationWebsite { get; set; }

		// User Information
		[Required]
		public Guid UserId { get; set; }

		[Required]
		[MaxLength(100)]
		public string UserFirstName { get; set; }

		[Required]
		[MaxLength(100)]
		public string UserLastName { get; set; }

		[Required]
		[EmailAddress]
		public string UserEmail { get; set; }

		[Required]
		public UserRole UserRole { get; set; } = UserRole.Participant;

		[MaxLength(255)]
		public string? UserProfilePicture { get; set; }

		// Relationship metadata
		[Required]
		public bool IsOrganizationAdmin { get; set; } = false;

		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

		// Navigation properties
		[ForeignKey("OrganizationId")]
		public virtual Organization Organization { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }

	}
}