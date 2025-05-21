using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Flow
{
    // For creating a new Flow
    public class FlowCreateDto
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        public string? Description { get; set; }
        public required bool MultipleRuns { get; set; }

		[Required]
        public required Guid CreatedBy { get; set; }
    }

}
