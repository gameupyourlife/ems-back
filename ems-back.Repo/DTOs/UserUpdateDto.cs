using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// For updating user profiles
	public class UserUpdateDto
	{
		[StringLength(100)]
		public string? FirstName { get; set; }

		[StringLength(100)]
		public string? LastName { get; set; }

		[Url]
		[StringLength(255)]
		public string? ProfilePicture { get; set; }

		[StringLength(100, MinimumLength = 8)]
		[DataType(DataType.Password)]
		public string? NewPassword { get; set; }
	}
}
