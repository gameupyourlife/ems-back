using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

	namespace ems_back.Repo.Models
	{
		[Table("Files")]
		public class EventFile
	{
			[Key]
			public string Id { get; set; }

			[Required]
			[MaxLength(255)]
			public string Url { get; set; }

			[Required]
			[MaxLength(50)]
			public FileType Type { get; set; }  // "image", "document", etc.

			[Required]
			public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

			[Required]
			public Guid UploadedBy { get; set; }

			[MaxLength(255)]
			public string OriginalName { get; set; }

			[MaxLength(50)]
			public string ContentType { get; set; }

			public long SizeInBytes { get; set; }

			// Navigation properties
			[ForeignKey("UploadedBy")]
			public virtual User Uploader { get; set; }
		}
	}

