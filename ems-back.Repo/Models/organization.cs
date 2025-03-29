﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models
{
	[Table("Organizations")]
	public class Organization
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		[MaxLength(255)]
		public string Name { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[Required]
		public Guid CreatedBy { get; set; }

		[Required]
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		[Required]
		public Guid UpdatedBy { get; set; }

		[MaxLength(500)]
		public string Address { get; set; }

		[Column(TypeName = "text")]
		public string Description { get; set; }

		[MaxLength(255)]
		public string ProfilePicture { get; set; }

		[MaxLength(255)]
		public string Website { get; set; }

		// Navigation properties
		[ForeignKey("CreatedBy")]
		public virtual User Creator { get; set; }

		[ForeignKey("UpdatedBy")]
		public virtual User Updater { get; set; }

		public virtual ICollection<User> Members { get; set; }
	}
}
