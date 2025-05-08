using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Actions.ActionModels
{
    public class DescriptionChangeModel
    {
        public ActionType ActionType => ActionType.ChangeDescription;
        public string NewDescription { get; set; }

    }
}
