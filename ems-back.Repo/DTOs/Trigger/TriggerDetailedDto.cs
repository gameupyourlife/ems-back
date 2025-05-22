using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Flow;

namespace ems_back.Repo.DTOs.Trigger
{
    // Detailed response DTO (with Flow information)
    public class TriggerDetailedDto : TriggerDto
    {
        public required FlowDto Flow { get; set; }
    }
}
