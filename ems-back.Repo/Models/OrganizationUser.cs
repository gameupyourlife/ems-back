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
		[Key, Column(Order = 0)]
		public Guid OrganizationId { get; set; }

		[Key, Column(Order = 1)]
		public Guid UserId { get; set; }

		[Required]
		public UserRole UserRole { get; set; } = UserRole.User;
		[Required]
		public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // for navigation:

        [ForeignKey("OrganizationId")]
		public virtual Organization Organization { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
	}
}