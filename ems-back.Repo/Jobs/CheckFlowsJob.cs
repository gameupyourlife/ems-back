using System;
using System.Linq;
using System.Threading.Tasks;
using ems_back.Repo.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ems_back.Repo.Jobs
{
    public class CheckFlowsJob : IJob
    {
        private readonly ApplicationDbContext _dbContext;

        public CheckFlowsJob(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                //direkt auf Action und Trigger Models mappen wegen JSON und direkt in mehrere Listen abhängig nach Action aufteilen
                var flows = await _dbContext.Flows 
                    .Where(f => f.IsActive)
                    .Include(f => f.Event)
                    .Include(f => _dbContext.Triggers
                        .Where(t => t.FlowId == f.FlowId)) // Falls keine Navigation Property vorhanden
                    .ToListAsync();

                foreach (var flow in flows)
                {
                    var trigger = await _dbContext.Triggers.FirstOrDefaultAsync(t => t.FlowId == flow.FlowId);
                    var eventData = flow.Event;

                    Console.WriteLine($"Flow: {flow.Name}, Trigger: {trigger.GetType().Name}, Event: {eventData.Title}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Quartz] Fehler beim Abrufen der Daten: {ex.Message}");
            }
        }


    }
}
