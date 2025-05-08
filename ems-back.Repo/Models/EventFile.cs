using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ems_back.Repo.Models.Types;

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
        public FileType Type { get; set; }

        [Required]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid UploadedBy { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public Guid EventId { get; set; }

        public long? SizeInBytes { get; set; }

        // for navigation:

        [ForeignKey("UploadedBy")]
        public virtual User Uploader { get; set; }

        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }
    }
}