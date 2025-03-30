using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// For updating an existing agenda entry
	public class AgendaEntryUpdateDto
	{
		[Required]
		public Guid Id { get; set; }

		[MaxLength(255)]
		public string Title { get; set; }

		public string Description { get; set; }

		public DateTime? Start { get; set; }

		public DateTime? End { get; set; }
	}
}
