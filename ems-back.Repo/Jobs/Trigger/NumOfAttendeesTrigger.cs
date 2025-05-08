using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public class NumOfAttendeesTrigger : ITrigger
    {
        public TriggerType TriggerType => TriggerType.NumOfAttendees;
        public NumOfAttendeesOperator Operator { get; set; }
        public NumOfAttendeesValueType ValueType { get; set; }
        public int Value { get; set; }
    }

}
