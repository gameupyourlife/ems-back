using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models.Types
{
    public record RoleUpdateResult(bool Success, string? ErrorMessage = null);

}