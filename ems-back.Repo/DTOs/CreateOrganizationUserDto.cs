using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	public class CreateOrganizationUserDto
	{
		public Guid OrganizationId { get; set; }
		public Guid UserId { get; set; }
		public UserRole UserRole { get; set; } = UserRole.Participant;
	}
}
