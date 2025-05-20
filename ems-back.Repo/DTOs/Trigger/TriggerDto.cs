using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Trigger
{
    public class TriggerDto
    {
        public Guid Id { get; set; }
        public TriggerType Type { get; set; }
        public JsonElement Details { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? FlowId { get; set; }
        public Guid? FlowTemplateId { get; set; }
        public String? Name { get; set; }
        public String? Summary { get; set; }
    }
}
