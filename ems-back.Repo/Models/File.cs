using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Models
{
    [Table("Files")]
    public class EventFile
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrganizationId { get; set; }

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

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
    }
}

