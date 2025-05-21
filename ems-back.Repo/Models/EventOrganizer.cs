using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models
{
    [Table("EventOrganizers")]
    public class EventOrganizer
    {
        [Key, Column(Order = 0)]
        public required Guid EventId { get; set; }

        [Key, Column(Order = 1)]
        public required Guid UserId { get; set; }

        // for navigation:

        [ForeignKey("EventId")]
        public virtual  Event? Event { get; set; }

        [ForeignKey("UserId")]
        public virtual  User? User { get; set; }
    }
}
