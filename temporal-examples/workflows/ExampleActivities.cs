using Microsoft.Extensions.Logging;
using Temporalio.Activities;

namespace workflows;

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
        await Task.Delay(2000);
        _logger.LogInformation("Generic task completed.");
        return $"generic-task-{DateTime.Now}";
    }

    [Activity]
    public List<string> GenerateChildWorkflowsName()
    {
        int numberOfChildWorkflows = new Random().Next(1, 10);
        List<string> childWorkflowsInfo = [];
        for (int index = 0; index < numberOfChildWorkflows; index++)
        {
            _logger.LogInformation("Starting child workflow");
            childWorkflowsInfo.Add($"child-{index}-workflow-{DateTime.Now}");
        }
        return childWorkflowsInfo;
    }
}
