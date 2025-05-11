using ems_back.Repo.DTOs.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Repository
{
    public interface IEventFlowRepository
    {
        Task<IEnumerable<FlowOverviewDto>> GetAllFlows(Guid orgId, Guid eventId);
        Task<Guid> CreateFlow(FlowCreateDto flowDto);
        Task<FlowDto> GetFlowById(Guid orgId, Guid eventId, Guid flowId);
        Task<bool> DeleteFlow(Guid orgId, Guid eventId, Guid flowId);
        Task<FlowDto> UpdateFlow(Guid orgId, Guid eventId, Guid flowId, FlowDto flowDto);
    }
}
