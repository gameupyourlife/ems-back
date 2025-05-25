using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Jobs.Trigger;
using ems_back.Repo.Data;
using System.Text.Json.Serialization;
using ems_back.Repo.Jobs.Mapping.Triggers; // oder Newtonsoft.Json

namespace ems_back.Repo.Jobs
{
    public class MapTriggers
    {
        private readonly ApplicationDbContext _dbContext;

        public MapTriggers(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<BaseTrigger>> GetMappedTriggersAsync()
        {
            var triggers = await _dbContext.Set<Models.Trigger>()
                .Include(t => t.Flow)
                .Where(t => t.FlowId != null &&
                            t.Flow != null && // Ensure Flow is not null before accessing its properties
                            !t.Flow.IsActive &&
                            t.Flow.stillPending)
                .OrderBy(t => t.FlowId)
                .ToListAsync();

            Console.WriteLine($"[MapTriggers] Found {triggers.Count} triggers to map.");

            var result = new List<BaseTrigger>();

            foreach (var trigger in triggers)
            {
                BaseTrigger mapped = trigger.Type switch
                {
                    TriggerType.Date => DateTriggerMapping.MapDateTrigger(trigger),
                    TriggerType.RelativeDate => RelativeDateTriggerMapping.MapRelativeDateTrigger(trigger),
                    TriggerType.NumOfAttendees => NumOfAttendeesTriggerMapping.MapNumOfAttendeesTrigger(trigger),
                    TriggerType.Status => StatusTriggerMapping.MapStatusTrigger(trigger),
                    _ => throw new InvalidOperationException($"Unhandled TriggerType: {trigger.Type}")
                };

                result.Add(mapped);
            }

            return result;
        }
    }
}
