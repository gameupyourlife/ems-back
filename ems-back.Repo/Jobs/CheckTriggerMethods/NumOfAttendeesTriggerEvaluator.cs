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
    public class NumOfAttendeesTriggerEvalator
    {

        private readonly ApplicationDbContext _dbContext;

        public NumOfAttendeesTriggerEvalator(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsNumOfAttendeesTriggerMetAsync(NumOfAttendeesTrigger trigger)
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
    }
}
