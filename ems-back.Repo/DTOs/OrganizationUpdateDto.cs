using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// For updating organizations
	public class OrganizationUpdateDto
	{
		[StringLength(255)]
		public string? Name { get; set; }

		[StringLength(500)]
		public string? Address { get; set; }

		public string? Description { get; set; }

		[Url]
		[StringLength(255)]
		public string? Website { get; set; }

		[Url]
		[StringLength(255)]
		public string? ProfilePicture { get; set; }
	}
}
