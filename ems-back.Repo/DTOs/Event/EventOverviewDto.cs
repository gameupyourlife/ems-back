using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Event
{
    public class EventOverviewDto
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime Start { get; set; }
        public string Location { get; set; }
        public int Attendees { get; set; }
        public EventStatus Status { get; set; }
        public string? Description { get; set; }
    }
}
