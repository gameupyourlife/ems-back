using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.User;

namespace ems_back.Repo.DTOs.Organization
{
    // For API responses
    public class OrganizationResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Website { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Flattened user info
        public UserDto Creator { get; set; }
        public UserDto Updater { get; set; }

        // Counts instead of full collections
        public int MemberCount { get; set; }

        // Domain information
        public List<string> Domains { get; set; } = new List<string>();

        // Additional calculated properties
        public string PrimaryDomain => Domains?.FirstOrDefault() ?? string.Empty;
        public bool HasMultipleDomains => Domains?.Count > 1;
	}
}
