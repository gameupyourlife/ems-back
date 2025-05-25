using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Jobs.Trigger;

namespace ems_back.Repo.Jobs.CheckTriggerMethods
{
    public class DateTriggerEvaluator 
    {
        public bool IsDateTriggerMet(DateTrigger trigger)
        {
            var now = DateTime.UtcNow;

            return trigger.Operator switch
            {
                DateTriggerOperator.Before => now < trigger.Value,
                DateTriggerOperator.After => now > trigger.Value,
                DateTriggerOperator.On => now >= trigger.Value,
                _ => throw new InvalidOperationException($"Unbekannter Operator: {trigger.Operator}")
            };
        }
    }
}
