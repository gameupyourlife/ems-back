using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Trigger
{

    public enum NumOfAttendeesOperator
    {
        GreaterThan,
        LessThan,
        EqualTo
    }

    public enum NumOfAttendeesValueType
    {
        Absolute,
        Percentage
    }

    public class NumOfAttendeesTrigger : BaseTrigger
    {
        [JsonPropertyName("operator")]
        public required String Operator { get; set; }
        [JsonPropertyName("valueType")]
        public required String ValueType { get; set; }
        [JsonPropertyName("value")]
        public required int Value { get; set; }
    }

}
