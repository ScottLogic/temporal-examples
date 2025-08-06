using Microsoft.Extensions.Logging;
using Temporalio.Activities;

namespace Workflows;

public class ExampleActivities
{
    private readonly ILogger<ExampleActivities> _logger;
    public ExampleActivities(ILogger<ExampleActivities> logg)
    {
        _logger = logg;
    }

    [Activity]
    public async Task<string> GenericTask()
    {
        var duration = 2000;
        _logger.LogInformation($"Starting delay: {@duration}");
        await Task.Delay(duration);
        _logger.LogInformation("Finished delay");
        return $"generic-task-{DateTime.Now}";
    }

    [Activity]
    public List<string> GenerateChildWorkflowsName()
    {
        int numberOfChildWorkflows = new Random().Next(1, 10);
        List<string> childWorkflowsInfo = [];
        for (int index = 0; index < numberOfChildWorkflows; index++)
        {
            var name = $"child-{index}-workflow-{DateTime.Now}";
            _logger.LogInformation($"Creating child workflow {@name}");
            childWorkflowsInfo.Add(name);
        }
        return childWorkflowsInfo;
    }

    [Activity]
    public static async Task<string> TaskTriggeredBySignal()
    {
        await Task.Delay(2000);
        return $"task-triggered-by-signal-{DateTime.Now}";
    }

    [Activity]
    public static async Task<string> TaskTriggeredByTimeout()
    {
        await Task.Delay(2000);
        return $"task-triggered-by-timeout-{DateTime.Now}";
    }
}
