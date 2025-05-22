using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Models;
using System.Text.Json.Serialization;

namespace ems_back.Repo.Jobs.Trigger
{

    public enum DateTriggerOperator
    {
        Before,
        After,
        On
    }

    public class DateTrigger : BaseTrigger
    {
        [JsonPropertyName("operator")]
        public DateTriggerOperator Operator { get; set; }
        [JsonPropertyName("value")]
        public DateTime Value { get; set; }
    }
}
