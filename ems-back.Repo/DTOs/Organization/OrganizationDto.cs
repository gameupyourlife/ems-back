using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Organization
{
    public class OrganizationDto
    {
        public Guid OrgId { get; set; }
        public int NumOfMembers { get; set; }
        public int NumOfEvents { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string ProfilePicture { get; set; }
        public string Website { get; set; }
    }
}
