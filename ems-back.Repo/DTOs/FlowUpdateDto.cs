using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// For updating an existing Flow
	public class FlowUpdateDto
	{

		[Required]
		public Guid Id { get; set; }  // Add this line

		[Required]
		[MaxLength(100)]
		public string Name { get; set; }

		public string? Description { get; set; }

		[Required]
		public Guid UpdatedBy { get; set; }
	}
}
