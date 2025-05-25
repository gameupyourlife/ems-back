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
    public class ChangeTitleMapping
    {
        public static TitleChangeModel MapTitleChange(Models.Action action)
        {
            var details = JsonSerializer.Deserialize<TitleChangeModel>(action.Details)
                          ?? throw new InvalidOperationException("Invalid JSON for TitleChangeModel");

            return new TitleChangeModel
            {
                ActionId = action.Id,
                FlowId = action.FlowId ?? throw new InvalidOperationException("FlowId missing"),
                ActionType = ActionType.ChangeTitle,
                NewTitle = details.NewTitle
            };
        }
    }
}
