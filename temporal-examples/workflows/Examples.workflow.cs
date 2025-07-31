using Temporalio.Common;
using Temporalio.Workflows;

namespace workflows;

[Workflow]
public class ExampleWorkflow
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

        string result = await Workflow.ExecuteActivityAsync(
            (ExampleActivities a) => a.GenericTask(),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                RetryPolicy = retryPolicy,
            }
        );
        return "Workflow has run";
    }
}
