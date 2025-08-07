using Microsoft.Extensions.Logging;
using Temporalio.Activities;

namespace Workflows;

public class ExampleActivities
{
    private readonly ILogger<ExampleActivities> _logger;

    public ExampleActivities(ILogger<ExampleActivities> logger)
    {
        _logger = logger;
    }

    [Activity]
    public async Task<string> GenericTask()
    {
        var duration = 2000;
        _logger.LogInformation("Starting delay: {Duration}", duration);
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
            _logger.LogInformation("Creating child workflow {Name}", name);
            childWorkflowsInfo.Add(name);
        }
        return childWorkflowsInfo;
    }

    [Activity]
    public async Task<string> TaskTriggeredBySignal()
    {
        var duration = 2000;
        _logger.LogInformation("Starting delay: {Duration}", duration);
        await Task.Delay(duration);
        _logger.LogInformation("Finished delay");
        return $"task-triggered-by-signal-{DateTime.Now}";
    }

    [Activity]
    public async Task<string> TaskTriggeredByTimeout()
    {
        var duration = 2000;
        _logger.LogInformation("Starting delay: {Duration}", duration);
        await Task.Delay(duration);
        _logger.LogInformation("Finished delay");
        return $"task-triggered-by-timeout-{DateTime.Now}";
    }
}
