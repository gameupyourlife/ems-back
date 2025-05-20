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
                var count = await _dbContext.Flows.CountAsync();
                Console.WriteLine($"[Quartz] Es gibt aktuell {count} Flows in der Datenbank.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Quartz] Fehler beim Zählen der Flows: {ex.Message}");
            }
        }


    }
}
