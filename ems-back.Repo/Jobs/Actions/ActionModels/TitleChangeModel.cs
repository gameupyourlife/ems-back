using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Actions.ActionModels
{
    public class TitleChangeModel : IActionModel
    {
        public ActionType ActionType => ActionType.ChangeTitle;
        public required String NewTitle { get; set; }
    }
}
