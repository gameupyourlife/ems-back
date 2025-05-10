using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Event
{
    public class EventCreateDto
    {
        public string Title { get; set; }
        public Guid OrganizationId { get; set; } // evtl. nicht notwendig -> kommt aus Rute
        public string Description { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string Image { get; set; }
        public EventStatus Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public Guid CreatedBy { get; set; }
        public int MaxAttendees { get; set; } // Muss weg
    }
}