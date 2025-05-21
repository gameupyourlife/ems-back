using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Event;

namespace ems_back.Repo.DTOs.Agenda
{
    public class AgendaEntryDto
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required DateTime Start { get; set; }
        public required DateTime End { get; set; }
        public required Guid EventId { get; set; }
    }
}
