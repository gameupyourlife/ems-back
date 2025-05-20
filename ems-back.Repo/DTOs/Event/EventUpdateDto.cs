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
        public string Title { get; set; }
        public string Location { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
        public int Capacity { get; set; }
        public EventStatus Status { get; set; } 
        public string Image { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
    }

}
