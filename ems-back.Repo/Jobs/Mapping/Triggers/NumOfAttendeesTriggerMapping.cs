using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ems_back.Repo.Jobs.Trigger;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Mapping.Triggers
{
    public class NumOfAttendeesTriggerMapping
    {
        public static NumOfAttendeesTrigger MapNumOfAttendeesTrigger(Models.Trigger trigger)
        {
            var details = JsonSerializer.Deserialize<NumOfAttendeesTrigger>(trigger.Details, JsonOptionsProvider.Options)
                          ?? throw new InvalidOperationException("Invalid RelativeDateTrigger JSON");

            return new NumOfAttendeesTrigger
            {
                Id = trigger.Id,
                FlowId = trigger.FlowId,
                Flow = trigger.Flow,
                TriggerType = TriggerType.NumOfAttendees,
                Operator = details.Operator,
                ValueType = details.ValueType,
                Value = details.Value
            };
        }
    }
}
