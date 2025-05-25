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
    public class RelativeDateTriggerMapping
    {
        public static RelativeDateTrigger MapRelativeDateTrigger(Models.Trigger trigger)
        {
            var details = JsonSerializer.Deserialize<RelativeDateTrigger>(trigger.Details, JsonOptionsProvider.Options)
                          ?? throw new InvalidOperationException("Invalid RelativeDateTrigger JSON");

            return new RelativeDateTrigger
            {
                Id = trigger.Id,
                FlowId = trigger.FlowId,
                Flow = trigger.Flow,
                TriggerType = TriggerType.RelativeDate,
                Operator = details.Operator,
                Value = details.Value,
                ValueType = details.ValueType,
                ValueRelativeTo = details.ValueRelativeTo,
                ValueRelativeOperator = details.ValueRelativeOperator
            };
        }
    }
}
