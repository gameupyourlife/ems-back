using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models
{
    public class MailTemplate
    {
        [Key]
        public Guid Id { get; set; } = new Guid();

        [Required]
        public string Name { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public string? Description { get; set; }

        [Required]
        public bool isUserCreated { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        // for navigation:

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
    }
}
