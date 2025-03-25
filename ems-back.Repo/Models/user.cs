using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models
{

	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(100)]
		public string LastName { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string PasswordHash { get; set; } // Store hashed passwords

		public int OrganizationId { get; set; }

		[Required]
		public UserRole Role { get; set; } = UserRole.Participant; // Default role

		public bool IsEmailConfirmed { get; set; } = false;  // Email verification
		public string EmailConfirmationToken { get; set; }


		[ForeignKey("OrganizationId")]
		public Organization Organization { get; set; }
	}

}
