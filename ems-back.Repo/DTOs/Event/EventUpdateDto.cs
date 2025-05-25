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
        public required string Title { get; set; }
        public required string Location { get; set; }
        public string? Description { get; set; }
        public required string Category { get; set; }
        public required int Capacity { get; set; }
        public required EventStatus Status { get; set; } 
        public required string Image { get; set; }
        public required DateTime Start { get; set; }
        public required DateTime End { get; set; }
    }
}
