using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ems_back.Repo.Jobs.Actions.ActionModels;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Mapping.Actions
{
    public class ChangeStatusMapping
    {
        public static StatusChangeModel MapStatusChange(Models.Action action)
        {
            var details = JsonSerializer.Deserialize<StatusChangeModel>(action.Details, JsonOptionsProvider.Options)
                          ?? throw new InvalidOperationException("Invalid JSON for StatusChangeModel");

            return new StatusChangeModel
            {
                ActionId = action.Id,
                FlowId = action.FlowId ?? throw new InvalidOperationException("FlowId missing"),
                ActionType = ActionType.ChangeStatus,
                NewStatus = details.NewStatus
            };
        }
    }
}
