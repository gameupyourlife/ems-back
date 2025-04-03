using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	public class UpdateOrganizationUserDto
	{
		public UserRole UserRole { get; set; }
		public bool IsOrganizationAdmin { get; set; }
	}
}
