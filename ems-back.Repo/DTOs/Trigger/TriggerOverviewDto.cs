using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Trigger
{
    public class TriggerOverviewDto
    {
        public Guid Id { get; set; }
        public String? Name { get; set; }
        public TriggerType Type { get; set; }
        public String? Description { get; set; }
    }
}
