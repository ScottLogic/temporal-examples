using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Temporalio.Extensions.Hosting;
using Temporalio.Client;
using Temporalio.Runtime;
using Temporalio.Worker;
using workflows;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
        .ConfigureLogging(ctx => ctx.AddSimpleConsole().SetMinimumLevel(LogLevel.Information))
        .ConfigureServices(ctx => 
            ctx.AddHostedTemporalWorker(
                clientTargetHost: "host.docker.internal:7233",
                clientNamespace: "default",
                taskQueue: "example")
            .AddScopedActivities<ExampleActivities>()
            .AddWorkflow<ExampleWorkflow>()
            .AddWorkflow<ExampleWithChildrenWorkflow>()
            .AddWorkflow<WaitingSignalWorkflow>()
        )
        .Build();

        await host.RunAsync();
    }
}