using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Flow
{
    // Basic response (without navigation properties)
    public class FlowBasicDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }

        public bool stillPending { get; set; } = false;
        public bool multipleRuns { get; set; } = false;
        public Guid OrganizationId { get; set; }
        public Guid EventId { get; set; } // <-- Important!
	}
}
