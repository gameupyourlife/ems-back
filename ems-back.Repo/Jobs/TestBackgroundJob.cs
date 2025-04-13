using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace ems_back.Repo.Jobs;

[DisallowConcurrentExecution]
public class TestBackgroundJob : IJob
{
    private readonly ILogger _logger;

    public TestBackgroundJob(ILogger<TestBackgroundJob> logger)
    {
        _logger = logger;
    }
    public Task Execute(IJobExecutionContext context)
	{
        // Your job logic here
        _logger.LogInformation($"Logging was successful on {DateTime.Now}");
		return Task.CompletedTask;
	}
}