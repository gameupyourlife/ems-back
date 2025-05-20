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
        public string Name { get; set; }

        public string? Description { get; set; }
        public bool stillPending { get; set; } = false;
        public bool multipleRuns { get; set; } = false;

		[Required]
        public Guid CreatedBy { get; set; }
    }

}
