﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models
{
	[Table("AgendaEntries")]
	public class AgendaEntry
	{
		[Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
		[MaxLength(255)]
		public required string Title { get; set; }

		[Column(TypeName = "text")]
		public string? Description { get; set; }

		[Required]
		public required DateTime Start { get; set; }

		[Required]
		public required DateTime End { get; set; }

        [Required]
        public required Guid EventId { get; set; }

		// for navigation:

		[ForeignKey("EventId")]
		public virtual Event? Event { get; set; }
    }
}
