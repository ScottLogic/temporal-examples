using Microsoft.Extensions.Logging;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Workflows;

[Workflow]
public class WaitingSignalWorkflow
{
    private bool _shouldContinue = false;
    private TimeSpan _timeout = new(0, 0, 20);

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
        var activityOptions = new ActivityOptions()
        {
            StartToCloseTimeout = TimeSpan.FromMinutes(5),
            RetryPolicy = retryPolicy,
        };

        Workflow.Logger.LogInformation("Waiting for signal to continue");
        //var didNotTimeout = await Workflow.WaitConditionAsync(() => _shouldContinue, timeout: _timeout);
        // This is an experimental feature that allows to name the timer i.e "WaitingForSignal" instead of "WaitConditionAsync".
        // I left the stable version as well in case we want to revert back to it.
        var didNotTimeout = await Workflow.WaitConditionWithOptionsAsync(
            new WaitConditionOptions(() => _shouldContinue, _timeout, "WaitingForSignal")
        );

        if (didNotTimeout)
        {
            await Workflow.ExecuteActivityAsync(
                (ExampleActivities a) => a.TaskTriggeredBySignal(),
                activityOptions
            );
        }
        else
        {
            await Workflow.ExecuteActivityAsync(
                (ExampleActivities a) => a.TaskTriggeredByTimeout(),
                activityOptions
            );
        }

        return "Workflow has run";
    }

    [WorkflowSignal]
    public async Task Continue(string? name)
    {
        var message = name is null
            ? "Signal to continue has been received"
            : $"Signal to continue has been received by user: {name}";
        Workflow.Logger.LogInformation(message);
        _shouldContinue = true;
    }
}
