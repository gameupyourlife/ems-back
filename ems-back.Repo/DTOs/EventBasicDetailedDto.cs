using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
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
