using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Agenda;
using ems_back.Repo.DTOs.User;

namespace ems_back.Repo.DTOs.Event
{
    // Detailed response DTO
    public class EventBasicDetailedDto : EventBasicDto
    {
        public UserDto Creator { get; set; }
        public UserDto Updater { get; set; }
        public List<EventAttendeeDto> Attendees { get; set; } = new();
        public List<AgendaItemDto> AgendaItems { get; set; } = new();
    }

}
