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
    public class EmailMapping
    {
        public static EmailActionModel MapEmail(Models.Action action)
        {
            var details = JsonSerializer.Deserialize<EmailActionModel>(action.Details)
                          ?? throw new InvalidOperationException("Invalid JSON for EmailActionModel");

            return new EmailActionModel
            {
                ActionId = action.Id,
                FlowId = action.FlowId ?? throw new InvalidOperationException("FlowId missing"),
                ActionType = ActionType.SendEmail,
                MailId = (Guid)details.MailId
            };
        }
    }
}
