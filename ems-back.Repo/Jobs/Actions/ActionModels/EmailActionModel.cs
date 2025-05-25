using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Actions.ActionModels
{
    public class EmailActionModel : IActionModel
    {
        public ActionType ActionType => ActionType.SendEmail;
        public required string MailId { get; set; }
    }
}
