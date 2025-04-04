using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Action
{
    // Basic response DTO
    public class ActionDto
    {
        public Guid Id { get; set; }
        public ActionType Type { get; set; }
        public string Details { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? FlowId { get; set; }
    }
}
