using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ems_back.Repo.Models
{
	[Table("Organizations")]
	public class Organization
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		[MaxLength(255)]
		public string Name { get; set; }

		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[Required]
		public Guid CreatedBy { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

		public Guid? UpdatedBy { get; set; }

        [MaxLength(500)]
		public string? Address { get; set; }

		[Column(TypeName = "text")]
		public string? Description { get; set; }

		[MaxLength(255)]
		public string? ProfilePicture { get; set; }

		[Required]
		[MaxLength(255)]
		public string Domain { get; set; }

		// Navigation properties
		[ForeignKey("CreatedBy")]
		public virtual User Creator { get; set; }

		[ForeignKey("UpdatedBy")]
		public virtual User Updater { get; set; }

		public virtual ICollection<OrganizationUser> OrganizationUsers { get; set; } = new HashSet<OrganizationUser>();
	}
}