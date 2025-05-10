using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// Basic response DTO
	public class FileDto
	{
		public string Id { get; set; }
		public string Url { get; set; }
		public FileType Type { get; set; }
		public DateTime UploadedAt { get; set; }
		public Guid UploadedBy { get; set; }
		public string Name { get; set; }
	}
}
