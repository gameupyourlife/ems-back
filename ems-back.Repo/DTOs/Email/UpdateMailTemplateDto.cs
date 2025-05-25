using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Email
{
	public class UpdateMailTemplateDto
	{
		[StringLength(200)]
		public string? Name { get; set; }

		[StringLength(200)]
		public string? Subject { get; set; }

		public IEnumerable<Guid> Recipients { get; set; }

		public string? Body { get; set; }
		public string? Description { get; set; }

	}
}
