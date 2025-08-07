using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Temporalio.Extensions.Hosting;
using Temporalio.Runtime;
using Workflows;

namespace TemporalWorker.AutoFac.Modules
{
    public class TemporalWorkerConfigurationModule : Module
    {
        private readonly IConfiguration _configuration;

        public TemporalWorkerConfigurationModule(IConfiguration configuration)
        {
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override void Load(ContainerBuilder builder)
        {
            var services = new ServiceCollection();

            // Configure Temporal client with options from configuration
            var temporalConfig = _configuration.GetSection("Temporal");
            var clientHost = temporalConfig["ClientTargetHost"] ?? "host.docker.internal:7233";
            var clientNamespace = temporalConfig["Namespace"] ?? "default";
            // This is using a different queue to the other worker
            var taskQueue = temporalConfig["TaskQueue"] ?? "microservice-queue";

            services
                .AddTemporalClient(clientHost, clientNamespace)
                .Configure(options =>
                {
                    options.Runtime = new TemporalRuntime(
                        new()
                        {
                            Telemetry = new()
                            {
                                Metrics = new()
                                {
                                    Prometheus = new PrometheusOptions("0.0.0.0:9001"),
                                },
                            },
                        }
                    );
                });

            var workerBuilder = services
                .AddHostedTemporalWorker(taskQueue)
                .AddScopedActivities<MicroserviceActivities>();

            builder.Populate(services);
        }
    }
}
