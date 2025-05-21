using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Flow
{
    public class FlowOverviewDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required List<TriggerOverviewDto> Triggers { get; set; }
        public required List<ActionOverviewDto> Actions { get; set; }
    }
}