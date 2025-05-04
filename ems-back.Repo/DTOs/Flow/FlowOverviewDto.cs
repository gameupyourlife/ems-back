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
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid EventId { get; set; }
        public List<TriggerDto> Triggers { get; set; }
        public List<ActionDto> Actions { get; set; }
    }
}