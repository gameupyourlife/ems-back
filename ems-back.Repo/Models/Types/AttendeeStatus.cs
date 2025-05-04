using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models.Types
{
    public enum AttendeeStatus
    {
        Registered,
        Attended,
        NoShow,
        Cancelled,
        Waitlisted,
        Pending
    }
}