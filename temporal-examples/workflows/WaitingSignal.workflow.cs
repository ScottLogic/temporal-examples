using Microsoft.Extensions.Logging;
using Temporalio.Common;
using Temporalio.Workflows;

namespace workflows;

[Workflow]
public class WaitingSignalWorkflow
{
    private bool _shouldContinue = false;

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

        Workflow.Logger.LogInformation("Waiting for signal to continue");
        await Workflow.WaitConditionAsync(() => _shouldContinue);

        string result = await Workflow.ExecuteActivityAsync(
            () => ExampleActivities.GenericTask(),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                RetryPolicy = retryPolicy,
            }
        );
        return "Workflow has run";
    }

    [WorkflowSignal]
    public async Task Continue(string? name)
    {
        var message = name is null ? "Signal to continue has been received" : $"Signal to continue has been received by user: {name}";
        Workflow.Logger.LogInformation(message);
        _shouldContinue = true;
    }
}
