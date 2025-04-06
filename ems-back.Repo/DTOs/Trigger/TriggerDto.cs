using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Trigger
{
    // Basic response DTO
    public class TriggerDto
    {
        public Guid Id { get; set; }
        public TriggerType Type { get; set; }
        public string Details { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid FlowId { get; set; }

        public Guid OrganizationId { get; set; }
    }
}
