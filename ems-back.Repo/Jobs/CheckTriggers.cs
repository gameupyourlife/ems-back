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

            foreach (var trigger in triggers)
            {
                //bool isTriggered = false;

                switch (trigger.TriggerType)
                {
                    case TriggerType.Date:
                        if (trigger is DateTrigger dateTrigger)
                        {
                            if (IsDateTriggerMet(dateTrigger))
                            {
                                Console.WriteLine($"[DateTrigger] Flow {dateTrigger.FlowId} wird ausgelöst.");
                                // TODO: Flow starten
                            }
                            else
                            {
                                Console.WriteLine($"[DateTrigger] Noch nicht erfüllt: {dateTrigger.FlowId}");
                            }
                        }
                        break;
                    case TriggerType.NumOfAttendees
                    :
                        Console.WriteLine($"[NumOfAttendeesTrigger] Flow {trigger.FlowId} wird ausgelöst.");
                        break;
                    case TriggerType.Status:
                        Console.WriteLine($"[StatusTrigger] Flow {trigger.FlowId} wird ausgelöst.");
                        break;
                    case TriggerType.Registration:
                        Console.WriteLine($"[RegistrationTrigger] Flow {trigger.FlowId} wird ausgelöst.");
                        break;

                    case TriggerType.RelativeDate:
                        if (trigger is RelativeDateTrigger relativeTrigger)
                        {
                            if (await IsRelativeDateTriggerMetAsync(relativeTrigger))
                            {
                                Console.WriteLine($"[RelativeDateTrigger] Flow {relativeTrigger.FlowId} wird ausgelöst.");
                                // TODO: Flow starten
                            }
                            else
                            {
                                Console.WriteLine($"[RelativeDateTrigger] Noch nicht erfüllt: {relativeTrigger.FlowId}");
                            }
                        }
                        break;

                    /*case TriggerType.NumOfAttendees:
                        if (trigger is NumOfAttendeesTrigger numTrigger)
                        {
                            if (IsNumOfAttendeesTriggerMet(numTrigger))
                            {
                                Console.WriteLine($"[NumOfAttendeesTrigger] Flow {numTrigger.FlowId} wird ausgelöst.");
                            }
                            else
                            {
                                Console.WriteLine($"[NumOfAttendeesTrigger] Noch nicht erfüllt: {numTrigger.FlowId}");
                            }
                        }
                        break;

                    case TriggerType.Status:
                        if (trigger is StatusTrigger statusTrigger)
                        {
                            if (IsStatusTriggerMet(statusTrigger))
                            {
                                Console.WriteLine($"[StatusTrigger] Flow {statusTrigger.FlowId} wird ausgelöst.");
                            }
                            else
                            {
                                Console.WriteLine($"[StatusTrigger] Noch nicht erfüllt: {statusTrigger.FlowId}");
                            }
                        }
                        break;*/

                    default:
                        Console.WriteLine($"Unbekannter TriggerType: {trigger.TriggerType} für Trigger {trigger.Id}");
                        break;
                }
            }
            return await Task.FromResult(results);
        }


        // check if the trigger is met
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
            if (trigger.Flow == null || trigger.Flow.EventId == null)
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