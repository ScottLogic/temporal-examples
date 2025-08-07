using Temporalio.Activities;

namespace workflows;

// These activities can be calling an external microservice
// Arguably these could be added to the generic activities file but this does
// allow a separation.
public class MicroserviceActivities
{
    [Activity]
    public static async Task<bool> ValidateTransition(string transitionLocation)
    {
        await Task.Delay(1000);
        return true;
    }

    [Activity]
    public static async Task TransitionToApproved()
    {
        await Task.Delay(1500);
    }

    [Activity]
    public static async Task<Result<string>> CreateSecondaryCosts()
    {
        await Task.Delay(3000);
        return new Result<string> { value = "Created" };
    }

    [Activity]
    public static async Task<Result<string>> UpdateUpstream()
    {
        await Task.Delay(5000);
        return new Result<string> { value = "updated" };
    }

    public record Result<T>
    {
        public required T value;
        public readonly string? errorMessage;
    }
}
