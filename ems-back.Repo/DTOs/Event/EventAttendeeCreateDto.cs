using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Event
{
    public class EventAttendeeCreateDto
    {
        public Guid UserId { get; set; }
        public string ProfilePicture { get; set; }
    }
}
