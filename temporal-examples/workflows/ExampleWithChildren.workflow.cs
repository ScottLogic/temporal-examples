using Temporalio.Common;
using Temporalio.Workflows;

namespace workflows;

[Workflow]
public class ExampleWithChildrenWorkflow
{
    [WorkflowRun]
    public async Task<string> RunAsync()
    {
        // Retry policy
        var retryPolicy = new RetryPolicy
        {
            InitialInterval = TimeSpan.FromSeconds(1),
            MaximumInterval = TimeSpan.FromSeconds(100),
            BackoffCoefficient = 2,
            MaximumAttempts = 3,
        };

        List<string> childNames = await Workflow.ExecuteActivityAsync(
            () => ExampleActivities.GenerateChildWorkflowsName(),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                RetryPolicy = retryPolicy,
            }
        );

        foreach (var child in childNames)
        {
            await Workflow.ExecuteChildWorkflowAsync(
                (ExampleWorkflow wf) => wf.RunAsync(),
                new() { Id = child, TaskQueue = "example" }
            );
        }
        return "All child workflows created and run";
    }
}
