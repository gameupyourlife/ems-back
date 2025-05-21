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
        public required ActionType Type { get; set; }

        public required String? Details { get; set; }
        public required String Name { get; set; }
        public String? Description { get; set; }

    }
}
