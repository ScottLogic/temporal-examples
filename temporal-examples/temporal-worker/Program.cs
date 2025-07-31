using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Temporalio.Extensions.Hosting;
using TemporalWorker.AutoFac.Modules;
using workflows;

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
                    builder.RegisterModule(new LoggingModule(LogLevel.Information));
                    builder.RegisterModule(new TemporalWorkerConfigurationModule(context.Configuration));
                }
            )
            .Build();

        await host.RunAsync();
    }
}
