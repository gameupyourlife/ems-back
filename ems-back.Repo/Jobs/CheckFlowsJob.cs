using ems_back.Repo.Data;
using ems_back.Repo.Jobs.Trigger;
using Quartz;

namespace ems_back.Repo.Jobs
{
    public class CheckFlowsJob : IJob
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly MapTriggers _mapTriggers;

        public CheckFlowsJob(ApplicationDbContext dbContext, MapTriggers mapTriggers)
        {
            _dbContext = dbContext;
            _mapTriggers = mapTriggers;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var mappedTriggers = await _mapTriggers.GetMappedTriggersAsync();
                Console.WriteLine($"[Quartz] Anzahl gefundener Trigger: {mappedTriggers.Count}");

                foreach (var trigger in mappedTriggers)
                {
                    Console.WriteLine($"Trigger ID: {trigger.Id}");
                    Console.WriteLine($"Typ: {trigger.TriggerType}");
                    Console.WriteLine($"FlowId: {trigger.FlowId}");

                    switch (trigger)
                    {
                        case DateTrigger dt:
                            Console.WriteLine($"→ DateTrigger: Operator={dt.Operator}, Value={dt.Value}");
                            break;
                        case RelativeDateTrigger rdt:
                            Console.WriteLine($"→ RelativeDateTrigger: Operator={rdt.Operator}, Value={rdt.Value}, ValueType={rdt.ValueType}, ValueRelativeTo={rdt.ValueRelativeTo}, ValueRelativeOperator={rdt.ValueRelativeOperator}");
                            break;
                        case NumOfAttendeesTrigger nat:
                            Console.WriteLine($"→ NumOfAttendeesTrigger: Operator={nat.Operator}, ValueType={nat.ValueType}, Value={nat.Value}");
                            break;
                        case StatusTrigger st:
                            Console.WriteLine($"→ StatusTrigger: Operator={st.Operator}, Value={st.Value}");
                            break;
                        default:
                            Console.WriteLine("→ Unbekannter Trigger-Typ");
                            break;
                    }

                    Console.WriteLine(new string('-', 60));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Quartz] Fehler beim Abrufen der Flows: {ex.Message}");
            }
        }
    }
}
