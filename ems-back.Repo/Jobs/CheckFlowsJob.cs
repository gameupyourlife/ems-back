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

                // Check if trigger conditions are met
                var triggerResults = await _checkTriggers.CheckTriggersAsync(mappedTriggers);

                // Verarbeite nur getriggerte Flows
                var triggeredFlows = triggerResults
                    .Where(result => result.IsTriggered)
                    .Select(result => result.FlowId)
                    .ToList();

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
