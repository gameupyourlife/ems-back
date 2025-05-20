using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Trigger
{
    public class TriggerCreateDto
    {
        [Required]
        public TriggerType Type { get; set; }

        public String? Details { get; set; }

        public String? Name { get; set; }
        public String? Summary { get; set; }
    }
}
