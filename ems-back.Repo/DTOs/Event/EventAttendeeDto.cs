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
        public required Guid UserId { get; set; }
        public required string UserEmail { get; set; }
        public required string UserName { get; set; }
        public required AttendeeStatus Status { get; set; }
        public required DateTime RegisteredAt { get; set; }
    }
}
