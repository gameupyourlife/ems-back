using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models;

namespace ems_back.Repo.Interfaces.Repository
{
    public interface IFlowRunRepository
    {
        Task<List<FlowsRun>> GetAllForOrganization(Guid orgId);
        Task<List<FlowsRun>> GetByEvent(Guid orgId, Guid eventId);
    }
}
