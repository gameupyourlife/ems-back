using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	public class OrganizationUserDto
	{
		public Guid Id { get; set; }
		public Guid OrganizationId { get; set; }
		public string OrganizationName { get; set; }
		public Guid UserId { get; set; }
		public string UserFullName { get; set; }
		public string UserEmail { get; set; }
		public UserRole UserRole { get; set; }
		public bool IsOrganizationAdmin { get; set; }
		public DateTime JoinedAt { get; set; }
	}
}
