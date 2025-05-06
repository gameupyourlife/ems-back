using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models.Types
{
    public enum EventStatus
    {
        SCHEDULED,
        ONGOING,
        COMPLETED,
        CANCELLED,
        POSTPONED,
        DELAYED,
        ARCHIVED
    }
}