using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Actions.ActionModels
{
    internal interface IActionModel
    {
        public ActionType ActionType { get; }
    }
}
