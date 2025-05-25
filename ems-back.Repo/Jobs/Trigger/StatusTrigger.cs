using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Trigger
{
    public enum StatusTriggerValue
    {
        Active,
        Cancelled,
        Completed,
        Archived,
        Draft
    }
    public enum StausTriggerOperator
    {
        Is,
        IsNot
    }
    public class StatusTrigger : BaseTrigger
    {
        [JsonPropertyName("operator")]
        public StausTriggerOperator Operator { get; set; }
        [JsonPropertyName("value")]
        public StatusTriggerValue Value { get; set; }
    }

}
