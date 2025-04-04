using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Trigger
{
    // For updating an existing Trigger
    public class TriggerUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public TriggerType Type { get; set; }

        public string Details { get; set; }
    }
}
