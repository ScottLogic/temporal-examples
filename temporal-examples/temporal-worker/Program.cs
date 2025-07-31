using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Temporalio.Extensions.Hosting;
using workflows;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using AutoFac.Modules;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureContainer<ContainerBuilder>((context, builder) => {
            builder.RegisterModule(new LoggingModule(LogLevel.Information));
        })
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