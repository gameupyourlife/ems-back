using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Data;
using ems_back.Repo.Jobs.Trigger;
using ems_back.Repo.Models.Types;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Jobs.CheckTriggerMethods
{
    public class StatusTriggerEvaluator
    {
        private readonly ApplicationDbContext _dbContext;

        public StatusTriggerEvaluator(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsStatusTriggerMetAsync(StatusTrigger trigger)
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
