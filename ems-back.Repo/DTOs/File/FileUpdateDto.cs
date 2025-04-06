using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// For updating file metadata
	public class FileUpdateDto
	{
		[Required]
		public string Id { get; set; }

		[MaxLength(255)]
		public string OriginalName { get; set; }
	}

}
