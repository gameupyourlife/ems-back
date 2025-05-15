using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Organization
{
    public class OrganizationUserDto
    {
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }
        public UserRole UserRole { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
