using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Models
{
    [Table("Triggers")]
    public class Trigger
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        public TriggerType Type { get; set; } // Changed from `string` to `enum`

        [Column(TypeName = "jsonb")]
        public string Details { get; set; }  // JSON configuration

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relationships
        public Guid? FlowId { get; set; }

        [ForeignKey("FlowId")]
        public virtual Flow Flow { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
    }
}
