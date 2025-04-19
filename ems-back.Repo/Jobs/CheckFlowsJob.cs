using System;
using System.Linq;
using System.Threading.Tasks;
using ems_back.Repo.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ems_back.Repo.Jobs
{
    public class CheckTriggersJob : IJob
    {
        private readonly ApplicationDbContext _dbContext;

        public CheckTriggersJob(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var allTriggers = await _dbContext.Triggers.ToListAsync();

                Console.WriteLine($"[Quartz] Anzahl Trigger in DB: {allTriggers.Count} | {DateTime.Now}");

                if (!allTriggers.Any())
                {
                    Console.WriteLine("[Quartz] Keine Trigger in der Datenbank gefunden.");
                }
                else
                {
                    foreach (var trigger in allTriggers)
                    {
                        Console.WriteLine($"Trigger-Type: {trigger.Type}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Quartz] Fehler beim Abrufen der Trigger: {ex.Message}");
            }
        }
    }
}