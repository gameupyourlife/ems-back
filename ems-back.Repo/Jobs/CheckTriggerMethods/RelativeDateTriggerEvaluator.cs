using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Data;
using ems_back.Repo.Jobs.Trigger;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Jobs.CheckTriggerMethods
{
    public class RelativeDateTriggerEvaluator
    {

        private readonly ApplicationDbContext _dbContext;

        public RelativeDateTriggerEvaluator(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> IsRelativeDateTriggerMetAsync(RelativeDateTrigger trigger)
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
    }
}
