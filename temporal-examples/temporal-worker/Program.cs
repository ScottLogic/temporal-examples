using Microsoft.Extensions.Configuration;
using Temporalio.Client;
using Temporalio.Runtime;
using Temporalio.Worker;
using workflows;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables() // This line enables environment variable overrides
    .Build();

var temporalUrl = config["Temporal:ClientUrl"] ?? "localhost:7233";

var runtime = new TemporalRuntime(
    new()
    {
        Telemetry = new()
        {
            Metrics = new() { Prometheus = new PrometheusOptions("0.0.0.0:9000") },
        },
    }
);

var client = await TemporalClient.ConnectAsync(new(temporalUrl) { Runtime = runtime });

// Cancellation token to shutdown worker on ctrl+c
using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    tokenSource.Cancel();
    eventArgs.Cancel = true;
};

var activities = new ExampleActivities();

// Create a worker with the activity and workflow registered
using var worker = new TemporalWorker(
    client,
    new TemporalWorkerOptions(taskQueue: "example")
        .AddAllActivities(activities)
        .AddWorkflow<ExampleWorkflow>()
        .AddWorkflow<ExampleWithChildrenWorkflow>()
        .AddWorkflow<WaitingSignalWorkflow>()
);

Console.WriteLine("Running worker...");
try
{
    await worker.ExecuteAsync(tokenSource.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Worker cancelled");
}
