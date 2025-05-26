using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.User;

namespace ems_back.Repo.DTOs.Organization
{
    public class OrganizationResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    
        public UserDto Creator { get; set; }
        public UserDto Updater { get; set; }

    
        public int MemberCount { get; set; }


        public List<string> Domains { get; set; } = new List<string>();


        public string PrimaryDomain => Domains?.FirstOrDefault() ?? string.Empty;
        public bool HasMultipleDomains => Domains?.Count > 1;

	}
}
