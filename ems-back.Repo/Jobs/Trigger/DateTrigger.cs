using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Trigger
{

    public enum DateTriggerOperator
    {
        Before,
        After,
        On
    }

    public class DateTrigger : ITrigger
    {

        public TriggerType TriggerType => TriggerType.Date;
        public DateTime Value { get; set; }

        public DateTriggerOperator Operator { get; set; }

    }
}
