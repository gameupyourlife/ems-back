using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	public class FileCreateDto
	{
		[Required]
		[MaxLength(255)]
		public string Url { get; set; }

		[Required]
		public FileType Type { get; set; }

		[Required]
		public Guid UploadedBy { get; set; }

        [Required]
		public string Id { get; set; }

        [MaxLength(255)]
		public string OriginalName { get; set; }

		[MaxLength(50)]
		public string ContentType { get; set; }

		public long SizeInBytes { get; set; }
	}
}
