using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Organization
{
    // For creating new organizations
    public class OrganizationCreateDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        public string? Description { get; set; }

        [Url]
        [StringLength(255)]
        public string? Website { get; set; }

        public string? ProfilePicture { get; set; }
        [Required]
        public Guid CreatedBy { get; set; } // User ID creating the organization
    }

}
