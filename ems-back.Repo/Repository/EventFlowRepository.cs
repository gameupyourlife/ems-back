using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.Interfaces.Repository;

namespace ems_back.Repo.Repository
{
    public class EventFlowRepository : IEventFlowRepository
    {
        public Task<IEnumerable<FlowOverviewDto>> GetAllFlows(Guid orgId, Guid eventId)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateFlow(FlowCreateDto flowDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFlow(Guid orgId, Guid eventId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        public Task<FlowDto> GetFlowById(Guid orgId, Guid eventId, Guid flowId)
        {
            throw new NotImplementedException();
        }

        public Task<FlowDto> UpdateFlow(Guid orgId, Guid eventId, Guid flowId, FlowDto flowDto)
        {
            throw new NotImplementedException();
        }
    }
}