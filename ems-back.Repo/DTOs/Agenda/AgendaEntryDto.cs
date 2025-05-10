using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Event;

namespace ems_back.Repo.DTOs.Agenda
{
    // Response DTO
    public class AgendaEntryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid EventId { get; set; }
    }
}
