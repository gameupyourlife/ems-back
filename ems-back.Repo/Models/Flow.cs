﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models
{
    [Table("Flows")]
	public class Flow
	{

		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
		[MaxLength(100)]
		public string Name { get; set; } // Removed nullable (`?`)

		public string? Description { get; set; }

		[Required]
		public bool IsActive { get; set; } = true;

		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[Required]
		public Guid CreatedBy { get; set; }

		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		[Required]
		public Guid UpdatedBy { get; set; }

		// Navigation properties
		[ForeignKey("CreatedBy")]
		public virtual User Creator { get; set; }

		[ForeignKey("UpdatedBy")]
		public virtual User Updater { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        public virtual List<Trigger> Triggers { get; set; } = new();
		public virtual List<Action> Actions { get; set; } = new();
	}
}
