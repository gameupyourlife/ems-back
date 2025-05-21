using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Email
{
	public class MailTemplateResponseDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Subject { get; set; }
		public string? Description { get; set; }
		public DateTime CreatedAt { get; set; }
		public bool IsUserCreated { get; set; }
		public Guid OrganizationId { get; set; }

		// Optional: Include organization name
		public string? OrganizationName { get; set; }
	}
}
