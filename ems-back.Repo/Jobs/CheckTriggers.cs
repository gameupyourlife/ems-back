using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Jobs.Trigger;
using ems_back.Repo.Data;
using System.Text.Json.Serialization; // oder Newtonsoft.Json

namespace ems_back.Repo.Jobs
{
    public class CheckTriggers
    {
        private readonly ApplicationDbContext _dbContext;

        public CheckTriggers(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<(Guid FlowId, bool IsTriggered)>> CheckTriggersAsync(IEnumerable<BaseTrigger> triggers)
        {
            var results = new List<(Guid FlowId, bool IsTriggered)>();

            // Gruppiere nur Trigger mit gültiger FlowId
            var groupedByFlow = triggers
                .Where(t => t.FlowId.HasValue)
                .GroupBy(t => t.FlowId!.Value); // FlowId ist hier garantiert nicht null

            foreach (var group in groupedByFlow)
            {
                bool allMet = true;

                foreach (var trigger in group)
                {
                    bool isMet = trigger.TriggerType switch
                    {
                        TriggerType.Date => trigger is DateTrigger dt && IsDateTriggerMet(dt),
                        TriggerType.RelativeDate => trigger is RelativeDateTrigger rdt && await IsRelativeDateTriggerMetAsync(rdt),
                        TriggerType.NumOfAttendees => trigger is NumOfAttendeesTrigger nat && await IsNumOfAttendeesTriggerMetAsync(nat),
                        TriggerType.Status => trigger is StatusTrigger st && await IsStatusTriggerMetAsync(st),
                        _ => false
                    };

                    if (!isMet)
                    {
                        allMet = false;
                        break;
                    }
                }

                results.Add((group.Key, allMet)); // group.Key ist vom Typ Guid
            }

            return results;
        }


        //Methods to check if the triggers are met
        private static bool IsDateTriggerMet(DateTrigger trigger)
        {
            var now = DateTime.UtcNow; // oder DateTime.Now, je nach Kontext

            return trigger.Operator switch
            {
                DateTriggerOperator.Before => now < trigger.Value,
                DateTriggerOperator.After => now > trigger.Value,
                DateTriggerOperator.On => now >= trigger.Value,
                _ => throw new InvalidOperationException($"Unbekannter Operator: {trigger.Operator}")
            };
        }

        private async Task<bool> IsRelativeDateTriggerMetAsync(RelativeDateTrigger trigger)
        {
            if (trigger.Flow == null)
                throw new InvalidOperationException("Trigger.Flow oder EventId ist null.");

            var eventData = await _dbContext.Events
                .Where(e => e.Id == trigger.Flow.EventId)
                .Select(e => new { e.Start, e.End })
                .FirstOrDefaultAsync();

            if (eventData == null)
                throw new InvalidOperationException($"Event mit ID {trigger.Flow.EventId} nicht gefunden.");

            // Berechne den Referenzzeitpunkt (EventStart/EventEnd)
            var referenceTime = trigger.ValueRelativeTo switch
            {
                RelativeDateRelativeTo.EventStart => eventData.Start,
                RelativeDateRelativeTo.EventEnd => eventData.End,
                _ => throw new InvalidOperationException("Ungültiger Wert für ValueRelativeTo.")
            };

            // Berechne den Zielzeitpunkt
            var offset = trigger.ValueType switch
            {
                RelativeDateValueType.Hours => TimeSpan.FromHours(trigger.Value),
                RelativeDateValueType.Days => TimeSpan.FromDays(trigger.Value),
                RelativeDateValueType.Weeks => TimeSpan.FromDays(7 * trigger.Value),
                RelativeDateValueType.Months => TimeSpan.FromDays(30 * trigger.Value), // grobe Näherung
                _ => throw new InvalidOperationException("Ungültiger Wert für ValueType.")
            };

            var triggerTime = trigger.ValueRelativeOperator == RelativeDateRelativeComparison.Before
                ? referenceTime - offset
                : referenceTime + offset;

            var now = DateTime.UtcNow;

            // Fix: Ensure triggerTime is not null before accessing TotalMinutes
            if (!triggerTime.HasValue)
                throw new InvalidOperationException("TriggerTime konnte nicht berechnet werden.");

            return trigger.Operator switch
            {
                RelativeDateOperator.Before => now < triggerTime.Value,
                RelativeDateOperator.After => now > triggerTime.Value,
                RelativeDateOperator.Equal => now >= triggerTime.Value,
                _ => throw new InvalidOperationException("Unbekannter Operator.")
            };
        }

        private async Task<bool> IsNumOfAttendeesTriggerMetAsync(NumOfAttendeesTrigger trigger)
        {
            if (trigger.Flow == null)
                throw new InvalidOperationException("Flow darf nicht null sein.");

            var eventId = trigger.Flow.EventId;

            var ev = await _dbContext.Events
                .Where(e => e.Id == eventId)
                .Select(e => new { e.AttendeeCount, e.Capacity })
                .FirstOrDefaultAsync();

            if (ev == null)
                throw new InvalidOperationException("Event nicht gefunden.");

            var attendeeCount = ev.AttendeeCount;
            var capacity = ev.Capacity;

            int comparisonValue = trigger.ValueType switch
            {
                NumOfAttendeesValueType.Absolute => trigger.Value,
                NumOfAttendeesValueType.Percentage => (int)(capacity * trigger.Value / 100.0),
                _ => throw new InvalidOperationException($"Unbekannter ValueType: {trigger.ValueType}")
            };

            return trigger.Operator switch
            {
                NumOfAttendeesOperator.GreaterThan => attendeeCount > comparisonValue,
                NumOfAttendeesOperator.LessThan => attendeeCount < comparisonValue,
                NumOfAttendeesOperator.EqualTo => attendeeCount == comparisonValue,
                _ => throw new InvalidOperationException($"Unbekannter Operator: {trigger.Operator}")
            };
        }

        private async Task<bool> IsStatusTriggerMetAsync(StatusTrigger trigger)
        {
            if (trigger.Flow == null)
                throw new InvalidOperationException("Flow darf nicht null sein.");

            var eventId = trigger.Flow.EventId;

            // Hole den aktuellen Status des Events
            var eventStatus = await _dbContext.Events
                .Where(e => e.Id == eventId)
                .Select(e => e.Status)
                .FirstOrDefaultAsync();

            // Überprüfe, ob der Event überhaupt existiert
            if (!Enum.IsDefined(typeof(EventStatus), eventStatus))
                throw new InvalidOperationException("Event nicht gefunden oder ungültiger Status.");

            // Vergleiche gemäß Operator
            return trigger.Operator switch
            {
                StausTriggerOperator.Is => (int)eventStatus == (int)trigger.Value,
                StausTriggerOperator.IsNot => (int)eventStatus != (int)trigger.Value,
                _ => throw new InvalidOperationException($"Unbekannter Operator: {trigger.Operator}")
            };
        }

    }
}