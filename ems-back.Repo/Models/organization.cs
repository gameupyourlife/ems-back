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
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		[MaxLength(255)]
		public required string Name { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[Required]
		public Guid CreatedBy { get; set; }

		public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

		public Guid? UpdatedBy { get; set; }

        [MaxLength(500)]
		public string? Address { get; set; }

		[Column(TypeName = "text")]
		public string? Description { get; set; }

		[MaxLength(255)]
		public string? ProfilePicture { get; set; }

		// for navigation:

		[ForeignKey("CreatedBy")]
		public virtual User? Creator { get; set; }

		[ForeignKey("UpdatedBy")]
		public virtual User? Updater { get; set; }

		public ICollection<OrganizationDomain>? AllowedDomains { get; set; }
		public virtual ICollection<OrganizationUser> OrganizationUsers { get; set; } = new List<OrganizationUser>();
		public virtual ICollection<Event> Events { get; set; } = new List<Event>();
		public virtual ICollection<FlowTemplate> FlowTemplates { get; set; } = new List<FlowTemplate>();
        public virtual ICollection<MailTemplate> MailTemplates { get; set; } = new List<MailTemplate>();
    }
}