using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Action
{
    // For creating a new action
    public class ActionCreateDto
    {
        [Required]
        public ActionType Type { get; set; }

        [Required]
        public string Details { get; set; }

        public Guid? FlowId { get; set; }
    }
}
