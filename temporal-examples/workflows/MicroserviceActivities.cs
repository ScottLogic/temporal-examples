using Microsoft.Extensions.Logging;
using Temporalio.Activities;

namespace Workflows;

// These activities can be calling an external microservice
// Arguably these could be added to the generic activities file but this does
// allow a separation.
public class MicroserviceActivities
{
    private readonly ILogger<MicroserviceActivities> _logger;

    public MicroserviceActivities(ILogger<MicroserviceActivities> logger)
    {
        _logger = logger;
    }

    [Activity]
    public async Task<bool> ValidateTransition(string transitionLocation)
    {
        var duration = 2000;
        _logger.LogInformation("Starting delay: {Duration}", duration);
        await Task.Delay(1000);
        _logger.LogInformation("Finished delay");
        return true;
    }

    [Activity]
    public async Task TransitionToApproved()
    {
        ActivityExecutionContext.Current.Logger.LogInformation(
            "Executing TransitionToApproved activity for OpenTelemetry sample."
        );
        var duration = 1500;
        _logger.LogInformation("Starting delay: {Duration}", duration);
        await Task.Delay(1500);
        _logger.LogInformation("Finished delay");
    }

    [Activity]
    public async Task<Result<string>> CreateSecondaryCosts()
    {
        ActivityExecutionContext.Current.Logger.LogInformation(
            "Executing CreateSecondaryCosts activity for OpenTelemetry sample."
        );
        var duration = 3000;
        _logger.LogInformation("Starting delay: {Duration}", duration);
        await Task.Delay(duration);
        _logger.LogInformation("Finished delay");
        return new Result<string> { Value = "Created" };
    }

    [Activity]
    public async Task<Result<string>> UpdateUpstream()
    {
        ActivityExecutionContext.Current.Logger.LogInformation(
            "Executing UpdateUpstream activity for OpenTelemetry sample."
        );
        var duration = 5000;
        _logger.LogInformation("Starting delay: {Duration}", duration);
        await Task.Delay(duration);
        _logger.LogInformation("Finished delay");
        return new Result<string> { Value = "updated" };
    }

    public record Result<T>
    {
        public required T Value;
        public string? ErrorMessage { get; init; }
    }
}
