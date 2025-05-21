using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Action
{
    // For creating a new action
    public class ActionCreateDto
    {
        [Required]
        public ActionType Type { get; set; }

        [Required]
        public String? Details { get; set; }
        public String Name { get; set; }
        public String? Description { get; set; }

    }
}
