using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Flow
{
    // For updating an existing Flow
    public class FlowUpdateDto
    {

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public Guid UpdatedBy { get; set; }

        public bool stillPending { get; set; } = false;
        public bool multipleRuns { get; set; } = false;
	}
}
