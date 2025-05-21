using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Email
{
	public class CreateMailTemplateDto
	{
		[Required]
		[StringLength(200)]
		public string Name { get; set; }

		[Required]
		[StringLength(200)]
		public string Subject { get; set; }

		[Required]
		public string Body { get; set; }

		public string? Description { get; set; }
	}
}
