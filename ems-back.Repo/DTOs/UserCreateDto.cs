using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// For creating new users (API input)
	public class UserCreateDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[StringLength(100)]
		public string FirstName { get; set; }

		[Required]
		[StringLength(100)]
		public string LastName { get; set; }

		[Required]
		public Guid OrganizationId { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 6)]
		public string Password { get; set; } // Only for creation
	}
}
