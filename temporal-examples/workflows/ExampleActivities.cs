using System;
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
}
