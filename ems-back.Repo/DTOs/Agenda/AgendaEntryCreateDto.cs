using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Agenda
{
    public class AgendaEntryCreateDto
    {
        [MaxLength(255)]
        public required string Title { get; set; }

        public string? Description { get; set; }

        public required DateTime Start { get; set; }

        public required DateTime End { get; set; }
    }
}
