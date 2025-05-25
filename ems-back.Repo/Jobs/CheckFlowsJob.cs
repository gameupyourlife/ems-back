using ems_back.Repo.Data;
using ems_back.Repo.Jobs.Actions.ActionModels;
using ems_back.Repo.Jobs.Trigger;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Models;
using Quartz;

namespace ems_back.Repo.Jobs
{
    public class CheckFlowsJob : IJob
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly MapTriggers _mapTriggers;
        private readonly CheckTriggers _checkTriggers;
        private readonly MapActions _mapActions;
        private readonly ProcessActionsForFlow _processActionsForFlow;

        public CheckFlowsJob(ApplicationDbContext dbContext, MapTriggers mapTriggers, CheckTriggers checkTriggers, 
            MapActions mapActions, ProcessActionsForFlow processActionsForFlow)
        {
            _dbContext = dbContext;
            _mapTriggers = mapTriggers;
            _checkTriggers = checkTriggers;
            _mapActions = mapActions;
            _processActionsForFlow = processActionsForFlow;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var mappedTriggers = await _mapTriggers.GetMappedTriggersAsync();

                var mappedActions = await _mapActions.GetMappedActionsAsync();

                // Log the details of each trigger
                // Vor Abgabe noch alles entfernen, was nicht für die Produktion benötigt wird
                foreach (var trigger in mappedTriggers)
                {
                    Console.WriteLine($"[Quartz] Trigger ID: {trigger.Id}");
                    Console.WriteLine($"Trigger ID: {trigger.Id}");
                    Console.WriteLine($"Typ: {trigger.TriggerType}");
                    Console.WriteLine($"FlowId: {trigger.FlowId}");

                    Console.WriteLine(new string('-', 60));
                }

                // Log the details of each trigger
                // Vor Abgabe noch alles entfernen, was nicht für die Produktion benötigt wird
                foreach (var action in mappedActions)
                {
                    Console.WriteLine($"[Quartz] Action ID: {action.ActionId}");
                    Console.WriteLine($"Trigger ID: {action.ActionId}");
                    Console.WriteLine($"Typ: {action.ActionType}");
                    Console.WriteLine($"FlowId: {action.FlowId}");

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
                    var flowRun = new FlowsRun
                    {
                        Id = Guid.NewGuid(),
                        FlowId = flowId,
                        Status = FlowRunStatus.Pending,
                        Timestamp = DateTime.UtcNow,
                        Logs = $"Flow [{flowId}] wurde um {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} der Flows-Queue hinzugefügt."
                    };

                    _dbContext.FlowsRun.Add(flowRun);
                    await _dbContext.SaveChangesAsync();
                }

                foreach (var flowId in triggeredFlows)
                {
                    await _processActionsForFlow.ProcessActionsForFlowAsync(flowId, mappedActions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Quartz] Fehler beim Abrufen der Flows: {ex.Message}");
            }
        }
    }
}
