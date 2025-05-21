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
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ActionType Type { get; set; }
        public string? Description { get; set; }
    }
}
