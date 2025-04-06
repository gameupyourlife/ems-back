using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Action
{
    // For updating an existing action
    public class ActionUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        public ActionType? Type { get; set; }
        public string Details { get; set; }
    }
}
