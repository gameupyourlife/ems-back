using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Flow.FlowsRun;

namespace ems_back.Repo.Interfaces.Service
{
    public interface IFlowRunService
    {
        Task<List<FlowsRunResponseDto>> GetAllForOrganizationAsync(Guid orgId);
        Task<List<FlowsRunResponseDto>> GetByEventAsync(Guid orgId, Guid eventId);
    }
}
