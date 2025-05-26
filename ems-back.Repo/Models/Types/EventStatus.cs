using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models.Types
{
    public enum EventStatus
    {
        Scheduled,
        Ongoing,
        Completed,
        Cancelled,
        Postponed,
        Delayed,
        Archived
    }
}