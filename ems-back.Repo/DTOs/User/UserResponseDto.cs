using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.User
{

    public class UserResponseDto : UserDto
    {
        public DateTime CreatedAt { get; set; }
       public OrganizationOverviewDto Organization { get; set; }
        public UserRole Role { get; set; }
    }

}
