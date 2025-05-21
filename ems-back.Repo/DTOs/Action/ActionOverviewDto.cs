using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Action
{
    public class ActionOverviewDto
    {
        public required Guid Id { get; set; }
        public required String? Name { get; set; }
        public required ActionType Type { get; set; }
        public string? Description { get; set; }
    }
}
