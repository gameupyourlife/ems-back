using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using System.Text.Json;

namespace ems_back.Repo.Jobs.Trigger
{
    [Table("Triggers")]
    public abstract class BaseTrigger
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public TriggerType TriggerType { get; set; }
        public Guid? FlowId { get; set; }
        public JsonElement Details { get; set; }    
       
        [ForeignKey("FlowId")]
        public virtual Flow? Flow { get; set; }
    }
}
