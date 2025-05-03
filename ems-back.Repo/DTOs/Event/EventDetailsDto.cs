using ems_back.Repo.DTOs.Agenda;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Event
{
    public class EventDetailsDto
    {
        public EventInfoDTO Metadata { get; set; }
        public OrganizationDto Organization { get; set; } // Organization Dto überarbeiten
        public List<UserDto> Attendees { get; set; } // UserDto überarbeiten
        public List<FlowBasicDto> Flows { get; set; } // FlowDto überarbeiten
        public List<AgendaItemDto> Agenda { get; set; } // AgendaItemDto überarbeiten
        public List<EmailDto> Emails { get; set; }
    }
}
