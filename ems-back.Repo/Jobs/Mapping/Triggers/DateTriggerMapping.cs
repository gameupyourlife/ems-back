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
    public class DateTriggerMapping
    {
        public static DateTrigger MapDateTrigger(Models.Trigger trigger)
        {
            var details = JsonSerializer.Deserialize<DateTrigger>(trigger.Details, JsonOptionsProvider.Options)
                  ?? throw new InvalidOperationException("Invalid DateTrigger JSON");

            return new DateTrigger
            {
                Id = trigger.Id,
                FlowId = trigger.FlowId,
                Flow = trigger.Flow,
                TriggerType = TriggerType.Date,
                Operator = details.Operator,
                Value = details.Value
            };
        }
    }
}
