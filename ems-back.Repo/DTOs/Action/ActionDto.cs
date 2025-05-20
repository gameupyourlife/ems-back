using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Action
{
    // Basic response DTO
    public class ActionDto
    {
        public Guid Id { get; set; }
        public ActionType Type { get; set; }
        public JsonElement Details { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? FlowId { get; set; }
        public Guid? FlowTemplateId { get; set; }
        public String Name { get; set; }
        public String? Summary { get; set; }

    }
}
