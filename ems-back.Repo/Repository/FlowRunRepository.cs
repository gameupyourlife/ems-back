using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ems_back.Repo.Data;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Models;

namespace ems_back.Repo.Repository
{
    public class FlowRunRepository : IFlowRunRepository
    {
        private readonly ApplicationDbContext _context;

        public FlowRunRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FlowsRun>> GetAllForOrganization(Guid orgId)
        {
            // Alle Event-IDs der Organisation holen
            var eventIds = await _context.Events
                .Where(e => e.OrganizationId == orgId)
                .Select(e => e.Id)
                .ToListAsync();

            // Alle Flow-IDs dieser Events holen
            var flowIds = await _context.Flows
                .Where(f => eventIds.Contains(f.EventId))
                .Select(f => f.EventId)
                .ToListAsync();

            // Alle FlowRuns zu diesen Flows holen
            return await _context.FlowsRun
                .Where(fr => flowIds.Contains(fr.FlowId))
                .Include(fr => fr.Flow)
                .ThenInclude(f => f.Event)
                .ToListAsync();
        }

        public async Task<List<FlowsRun>> GetByEvent(Guid orgId, Guid eventId)
        {
            // Sicherstellen, dass das Event zur Organisation gehört
            var validEvent = await _context.Events
                .AnyAsync(e => e.Id == eventId && e.OrganizationId == orgId);

            if (!validEvent)
                return new List<FlowsRun>();

            // Alle Flow-IDs zu diesem Event
            var flowIds = await _context.Flows
                .Where(f => f.EventId == eventId)
                .Select(f => f.EventId)
                .ToListAsync();

            // FlowRuns zu diesen Flows holen
            return await _context.FlowsRun
                .Where(fr => flowIds.Contains(fr.FlowId))
                .Include(fr => fr.Flow)
                .ThenInclude(f => f.Event)
                .ToListAsync();
        }
    }
}
