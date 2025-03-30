using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// For API responses (extends UserDto)
	public class UserResponseDto : UserDto
	{
		public DateTime CreatedAt { get; set; }
		public OrganizationDto Organization { get; set; }
		public UserRole Role { get; set; }
	}

}
