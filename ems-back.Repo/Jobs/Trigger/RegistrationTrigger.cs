using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Trigger
{
    public class RegistrationTrigger : BaseTrigger
    {
       public new static TriggerType TriggerType => TriggerType.Registration;
    }
}
