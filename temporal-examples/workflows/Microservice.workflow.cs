using Temporalio.Common;
using Temporalio.Workflows;
using Workflows;

namespace Workflows;

[Workflow]
public class MicroservicesWorkflow
{
    // This workflow has activities that run on different workers. Anything running via the MicroserviceActivities
    // runs on a different worker.
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

        await Workflow.ExecuteActivityAsync(
            (ExampleActivities a) => a.GenericTask(),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                RetryPolicy = retryPolicy,
            }
        );

        bool result = await Workflow.ExecuteActivityAsync(
            () => MicroserviceActivities.ValidateTransition("Transition to validate"),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                RetryPolicy = retryPolicy,
                TaskQueue = "microservice-queue",
            }
        );

        await Workflow.ExecuteActivityAsync(
            () => MicroserviceActivities.CreateSecondaryCosts(),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                RetryPolicy = retryPolicy,
                TaskQueue = "microservice-queue",
            }
        );

        var updateUpstream = Workflow.ExecuteActivityAsync(
            () => MicroserviceActivities.UpdateUpstream(),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                RetryPolicy = retryPolicy,
                TaskQueue = "microservice-queue",
            }
        );

        var transitionToApproved = Workflow.ExecuteActivityAsync(
            () => MicroserviceActivities.TransitionToApproved(),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                RetryPolicy = retryPolicy,
                TaskQueue = "microservice-queue",
            }
        );

        await Workflow.WhenAllAsync(updateUpstream, transitionToApproved);

        return "Workflow has run";
    }
}
