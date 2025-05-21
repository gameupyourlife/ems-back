using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Action
{
    // For updating an existing action
    public class ActionUpdateDto
    {
        public required ActionType Type { get; set; }

        public required String Details { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }
    }
}
