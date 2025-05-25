using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using ems_back.Repo.Jobs.Actions.ActionModels;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Mapping.Actions
{
    public class ChangeDescriptionMapping
    {
        public static DescriptionChangeModel MapDescriptionChange(Models.Action action)
        {
            var details = JsonSerializer.Deserialize<DescriptionChangeModel>(action.Details)
                          ?? throw new InvalidOperationException("Invalid JSON for DescriptionChangeModel");

            return new DescriptionChangeModel
            {
                ActionId = action.Id,
                FlowId = action.FlowId ?? throw new InvalidOperationException("FlowId missing"),
                ActionType = ActionType.ChangeDescription,
                NewDescription = details.NewDescription
            };
        }
    }
}
