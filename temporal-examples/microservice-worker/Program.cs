using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared.AutoFac.Modules;
using Temporalio.Extensions.OpenTelemetry;
using TemporalWorker.AutoFac.Modules;

namespace TemporalWorker;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(
                (context, builder) =>
                {
                    builder.RegisterModule(new LoggingModule(context.Configuration));
                    builder.RegisterModule(
                        new TemporalWorkerConfigurationModule(context.Configuration)
                    );
                }
            )
            .Build();

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService("TemporalExamples.OpenTelemetry", serviceInstanceId: "microservice-worker");

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddSource(
                TracingInterceptor.ClientSource.Name,
                TracingInterceptor.WorkflowsSource.Name,
                TracingInterceptor.ActivitiesSource.Name
            )
            .AddOtlpExporter(o => o.Endpoint = new Uri("http://host.docker.internal:4317"))
            .Build();

        await host.RunAsync();
    }
}
