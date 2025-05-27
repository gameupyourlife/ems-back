using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Event
{
    public class EventOrganizerDto
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
    }
}
