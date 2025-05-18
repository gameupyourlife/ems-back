using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.DTOs.User
{
    public class UserUpdateRoleDto
    {
        public Guid userId { get; set; }
        public Guid? OrganizationId { get; set; }
        public UserRole newRole { get; set; }

    }
}
