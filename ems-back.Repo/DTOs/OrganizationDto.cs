using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// For minimal organization info (used in dropdowns/lists)
	public class OrganizationDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string? ProfilePicture { get; set; }
	}
}
