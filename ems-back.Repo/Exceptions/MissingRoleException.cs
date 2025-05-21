using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Exceptions
{
    public class MissingRoleException : Exception
    {
        public MissingRoleException(string message) : base(message)
        {
        }

    }
}
