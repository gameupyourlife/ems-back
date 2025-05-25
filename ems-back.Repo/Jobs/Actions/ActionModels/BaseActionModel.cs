using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Actions.ActionModels
{
    public abstract class BaseAction
    {
        public Guid ActionId { get; set; }
        public ActionType ActionType { get; set; }
        public required Guid FlowId { get; set; }

    }
}
