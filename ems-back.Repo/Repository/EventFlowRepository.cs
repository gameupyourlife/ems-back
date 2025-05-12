using AutoMapper;
using ems_back.Repo.Data;
using ems_back.Repo.DTOs.Action;
using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.DTOs.Trigger;
using ems_back.Repo.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Repository
{
    public class EventFlowRepository : IEventFlowRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EventFlowRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FlowOverviewDto>> GetAllFlows(Guid orgId, Guid eventId)
        {
            var flows = await _context.Flows
                .Where(f => f.EventId == eventId)
                .Include(e => e.Triggers)
                .Include(e => e.Actions)
                .Select(f => new FlowOverviewDto
                {
                    Id = f.FlowId,
                    Name = f.Name,
                    Description = f.Description,
                    Triggers = f.Triggers.Select(t => new TriggerOverviewDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Summary = t.Summary,
                        Type = t.Type,
                    }).ToList(),
                    Actions = f.Actions.Select(a => new ActionOverviewDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Summary = a.Summary,
                        Type = a.Type,
                    }).ToList(),

                })
                .AsNoTracking()
                .ToListAsync();

            return flows;
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