using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Identity;

namespace ems_back.Repo.Models
{
    [Table("Organisation_User")]
	public class OrganizationUser
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; } = Guid.NewGuid();

		// Organization Information
		[Required]
		public Guid OrganizationId { get; set; }

		// User Information
		[Required]
		public Guid UserId { get; set; }

		[Required]
		public UserRole UserRole { get; set; } = UserRole.Participant;

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