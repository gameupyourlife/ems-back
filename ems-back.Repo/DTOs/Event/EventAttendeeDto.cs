using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Event
{
    public class EventAttendeeDto
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public EventStatus Status { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
