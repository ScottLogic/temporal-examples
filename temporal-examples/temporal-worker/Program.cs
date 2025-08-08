using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared.AutoFac.Modules;
using Temporalio.Client;
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
                    builder.RegisterModule(
                        new TemporalWorkerConfigurationModule(context.Configuration)
                    );
                    builder.RegisterModule(new LoggingModule(context.Configuration));
                }
            )
            .Build();

        var assemblyName = typeof(TemporalClient).Assembly.GetName();

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService("TemporalExamples.OpenTelemetry", serviceInstanceId: "example");

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddSource(
                TracingInterceptor.ClientSource.Name,
                TracingInterceptor.WorkflowsSource.Name,
                TracingInterceptor.ActivitiesSource.Name
            )
            .AddOtlpExporter()
            .Build();

        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddMeter(assemblyName.Name!)
            .AddOtlpExporter()
            .Build();

        await host.RunAsync();
    }
}
