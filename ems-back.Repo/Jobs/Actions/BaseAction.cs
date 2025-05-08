using ems_back.Repo.Data;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Models;
using System.Reactive;
using ems_back.Repo.Jobs.Trigger;
using System.Text.Json;

public class BaseAction
{
    private readonly ApplicationDbContext _dbContext;

    public BaseAction(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// Logik für Überprüfung der Stati, die automatisch verläuft

    public async Task<FlowRunStatus> CreateFlowRunAsync(Guid flowId, FlowRunStatus status)
    {
        DateTime timestamp = DateTime.UtcNow;

        var flowRun = new FlowsRun
        {
            FlowId = flowId,
            Status = status,
            Timestamp = timestamp,
            Logs = $"FlowRun {status} at {timestamp}"
        };

        _dbContext.FlowsRun.Add(flowRun);
        await _dbContext.SaveChangesAsync();

        return status;
    }

    protected ITrigger ParseTrigger(string json, TriggerType triggerType)
    {
        return triggerType switch
        {
            TriggerType.Date => JsonSerializer.Deserialize<DateTrigger>(json),
            TriggerType.RelativeDate => JsonSerializer.Deserialize<RelativeDateTrigger>(json),
            TriggerType.NumOfAttendees => JsonSerializer.Deserialize<NumOfAttendeesTrigger>(json),
            TriggerType.Status => JsonSerializer.Deserialize<StatusTrigger>(json),
            TriggerType.Registration => JsonSerializer.Deserialize<RegistrationTrigger>(json),
            _ => throw new NotSupportedException($"Unsupported TriggerType: {triggerType}")
        };
    }
}
