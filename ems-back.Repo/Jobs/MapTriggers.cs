using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Jobs.Trigger;
using ems_back.Repo.Data;
using System.Text.Json.Serialization; // oder Newtonsoft.Json

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
                    TriggerType.Date => MapDateTrigger(trigger),
                    TriggerType.RelativeDate => MapRelativeDateTrigger(trigger),
                    TriggerType.NumOfAttendees => MapNumOfAttendeesTrigger(trigger),
                    TriggerType.Status => MapStatusTrigger(trigger),
                    _ => throw new InvalidOperationException($"Unhandled TriggerType: {trigger.Type}")
                };

                result.Add(mapped);
            }

            return result;
        }


        //Ensure that the JSON string is valid and matches the expected structure
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        //Map the trigger to the specific trigger type
        private static DateTrigger MapDateTrigger(Models.Trigger trigger)
        {
            var details = JsonSerializer.Deserialize<DateTrigger>(trigger.Details, _jsonOptions)
                  ?? throw new InvalidOperationException("Invalid DateTrigger JSON");

            return new DateTrigger
            {
                Id = trigger.Id,
                FlowId = trigger.FlowId,
                Flow = trigger.Flow,
                TriggerType = TriggerType.Date,
                Operator = details.Operator,
                Value = details.Value
            };
        }

        private static RelativeDateTrigger MapRelativeDateTrigger(Models.Trigger trigger)
        {
            var details = JsonSerializer.Deserialize<RelativeDateTrigger>(trigger.Details, _jsonOptions)
                          ?? throw new InvalidOperationException("Invalid RelativeDateTrigger JSON");

            return new RelativeDateTrigger
            {
                Id = trigger.Id,
                FlowId = trigger.FlowId,
                Flow = trigger.Flow,
                TriggerType = TriggerType.RelativeDate,
                Operator = details.Operator,
                Value = details.Value,
                ValueType = details.ValueType,
                ValueRelativeTo = details.ValueRelativeTo,
                ValueRelativeOperator = details.ValueRelativeOperator
            };
        }

        private static NumOfAttendeesTrigger MapNumOfAttendeesTrigger(Models.Trigger trigger)
        {
            var details = JsonSerializer.Deserialize<NumOfAttendeesTrigger>(trigger.Details, _jsonOptions)
                          ?? throw new InvalidOperationException("Invalid RelativeDateTrigger JSON");

            return new NumOfAttendeesTrigger
            {
                Id = trigger.Id,
                FlowId = trigger.FlowId,
                Flow = trigger.Flow,
                TriggerType = TriggerType.NumOfAttendees,
                Operator = details.Operator,
                ValueType = details.ValueType,
                Value = details.Value
            };
        }

        private static StatusTrigger MapStatusTrigger(Models.Trigger trigger)
        {
            var details = JsonSerializer.Deserialize<StatusTrigger>(trigger.Details, _jsonOptions)
                          ?? throw new InvalidOperationException("Invalid RelativeDateTrigger JSON");

            return new StatusTrigger
            {
                Id = trigger.Id,
                FlowId = trigger.FlowId,
                Flow = trigger.Flow,
                TriggerType = TriggerType.RelativeDate,
                Operator = details.Operator,
                Value = details.Value
            };
        }
    }
}
