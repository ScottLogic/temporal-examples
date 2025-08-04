using Temporalio.Activities;

namespace workflows;

public class ExampleActivities
{
    [Activity]
    public static async Task<string> GenericTask()
    {
        await Task.Delay(2000);
        return $"generic-task-{DateTime.Now}";
    }

    [Activity]
    public static List<string> GenerateChildWorkflowsName()
    {
        int numberOfChildWorkflows = new Random().Next(1, 10);
        List<string> childWorkflowsInfo = [];
        for (int index = 0; index < numberOfChildWorkflows; index++)
        {
            childWorkflowsInfo.Add($"child-{index}-workflow-{DateTime.Now}");
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
