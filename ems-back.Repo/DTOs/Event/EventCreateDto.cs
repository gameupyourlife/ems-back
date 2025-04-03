using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Event
{
    // For creating new events
    public class EventCreateDto
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        [Required]
        public Guid UpdatedBy { get; set; }

        public string Image { get; set; }
        public EventCategory? Category { get; set; }
        public EventStatus? Status { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
