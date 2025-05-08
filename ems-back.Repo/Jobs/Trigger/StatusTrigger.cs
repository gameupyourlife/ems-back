using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class StatusTrigger : ITrigger
    {
        public TriggerType TriggerType => TriggerType.Status;
        public StausTriggerOperator Operator { get; set; }
        public StatusTriggerValue Value { get; set; }
    }

}
