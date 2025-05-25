using ems_back.Repo.Data;
using ems_back.Repo.Jobs.Trigger;
using Quartz;

namespace ems_back.Repo.Jobs
{
    public class CheckFlowsJob : IJob
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly MapTriggers _mapTriggers;
        private readonly CheckTriggers _checkTriggers;

        public CheckFlowsJob(ApplicationDbContext dbContext, MapTriggers mapTriggers, CheckTriggers checkTriggers)
        {
            _dbContext = dbContext;
            _mapTriggers = mapTriggers;
            _checkTriggers = checkTriggers;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var mappedTriggers = await _mapTriggers.GetMappedTriggersAsync();
                
                // Log the details of each trigger
                // Vor Abgabe noch alles entfernen, was nicht für die Produktion benötigt wird
                foreach (var trigger in mappedTriggers)
                {
                    Console.WriteLine($"Trigger ID: {trigger.Id}");
                    Console.WriteLine($"Typ: {trigger.TriggerType}");
                    Console.WriteLine($"FlowId: {trigger.FlowId}");

                    switch (trigger)
                    {
                        case DateTrigger dt:
                            Console.WriteLine($"DateTrigger: Operator={dt.Operator}, Value={dt.Value}");
                            break;
                        case RelativeDateTrigger rdt:
                            Console.WriteLine($"RelativeDateTrigger: Operator={rdt.Operator}, Value={rdt.Value}, ValueType={rdt.ValueType}, ValueRelativeTo={rdt.ValueRelativeTo}, ValueRelativeOperator={rdt.ValueRelativeOperator}");
                            break;
                        case NumOfAttendeesTrigger nat:
                            Console.WriteLine($"NumOfAttendeesTrigger: Operator={nat.Operator}, ValueType={nat.ValueType}, Value={nat.Value}");
                            break;
                        case StatusTrigger st:
                            Console.WriteLine($"StatusTrigger: Operator={st.Operator}, Value={st.Value}");
                            break;
                        default:
                            Console.WriteLine("Unbekannter Trigger-Typ");
                            break;
                    }

                    Console.WriteLine(new string('-', 60));
                }

                // Check if trigger conditions are met
                var triggerResults = await _checkTriggers.CheckTriggersAsync(mappedTriggers);

                // Verarbeite nur getriggerte Flows
                var triggeredFlows = triggerResults
                    .Where(result => result.IsTriggered)
                    .Select(result => result.FlowId)
                    .ToList();

                Console.WriteLine($"[Quartz] Getriggerte Flows: {triggeredFlows.Count}");

                foreach (var flowId in triggeredFlows)
                {
                    Console.WriteLine($"Flow {flowId} wird ausgelöst.");

                    // TODO: Flow ausführen/starten (z.B. via EventFlowService)
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Quartz] Fehler beim Abrufen der Flows: {ex.Message}");
            }
        }
    }
}
