using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Temporalio.Extensions.OpenTelemetry;
using Temporalio.Client;
using Temporalio.Runtime;
using Temporalio.Worker;
using workflows;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Extensions.Logging;


var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables() // This line enables environment variable overrides
    .Build();

var temporalUrl = config["Temporal:ClientUrl"] ?? "localhost:7233";

// And update the tracer provider builder as follows:
using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(TracingInterceptor.ClientSource.Name, TracingInterceptor.WorkflowsSource.Name, TracingInterceptor.ActivitiesSource.Name)
    .AddConsoleExporter()
    .Build();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.File(
        new JsonFormatter(),
        "/logs/logstash.json",
        rollingInterval: RollingInterval.Day,
        shared: true
    )
    .CreateLogger();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSerilog(Log.Logger);
    builder.AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss]");
    builder.SetMinimumLevel(LogLevel.Information);
});

var runtime = new TemporalRuntime(
    new()
    {
        Telemetry = new()
        {
            Metrics = new() { Prometheus = new PrometheusOptions("0.0.0.0:9001") },
        },
    }
);

var client = await TemporalClient.ConnectAsync(new(temporalUrl) { Runtime = runtime, Interceptors = new[] { new TracingInterceptor() } , LoggerFactory = loggerFactory});

// Cancellation token to shutdown worker on ctrl+c
using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    tokenSource.Cancel();
    eventArgs.Cancel = true;
};

// This is definitelty not the best way of doing this, but for ease of simplicity rather than using a DI container, we will create the activities manually.
var activities = new ExampleActivities(loggerFactory.CreateLogger<ExampleActivities>());

// Create a worker with the activity and workflow registered
using var worker = new TemporalWorker(
    client,
    new TemporalWorkerOptions(taskQueue: "example")
        .AddAllActivities(activities)
        .AddWorkflow<ExampleWorkflow>()
        .AddWorkflow<ExampleWithChildrenWorkflow>()
);

var programLogger = loggerFactory.CreateLogger<Program>();

programLogger.LogInformation("Running worker...");
try
{
    await worker.ExecuteAsync(tokenSource.Token);
}
catch (OperationCanceledException)
{
    programLogger.LogInformation("Worker cancelled");
}
