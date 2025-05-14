using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.DTOs.Event
{
    public class EventUpdateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid OrganizationId { get; set; }
        public string Location { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
        public int Capacity { get; set; }
        public int AttendeeCount { get; set; }
        public EventStatus Status { get; set; }
        public string Image { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
