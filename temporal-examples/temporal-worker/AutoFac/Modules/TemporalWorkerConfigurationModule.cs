using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Temporalio.Client;
using Temporalio.Extensions.Hosting;
using Temporalio.Runtime;
using workflows;

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
            var taskQueue = temporalConfig["TaskQueue"] ?? "example";

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
                                    Prometheus = new PrometheusOptions("0.0.0.0:9000"),
                                },
                            },
                        }
                    );
                });

            var workerBuilder = services
                .AddHostedTemporalWorker(taskQueue)
                .AddScopedActivities<ExampleActivities>()
                .AddWorkflow<ExampleWorkflow>()
                .AddWorkflow<ExampleWithChildrenWorkflow>()
                .AddWorkflow<WaitingSignalWorkflow>();

            workerBuilder.ConfigureOptions(options =>
            {
                if (
                    int.TryParse(
                        temporalConfig["MaxConcurrentWorkflowTasks"],
                        out int maxWorkflowTasks
                    )
                )
                {
                    options.MaxConcurrentWorkflowTasks = maxWorkflowTasks;
                }

                if (
                    int.TryParse(
                        temporalConfig["MaxConcurrentActivityTasks"],
                        out int maxActivityTasks
                    )
                )
                {
                    options.MaxConcurrentActivities = maxActivityTasks;
                }
            });

            builder.RegisterType<ExampleActivities>().AsSelf().InstancePerLifetimeScope();
            builder
                .Register(ctx =>
                {
                    var options = ctx.Resolve<TemporalClientConnectOptions>();
                    return TemporalClient.CreateLazy(options);
                })
                .As<ITemporalClient>()
                .SingleInstance();

            builder.Populate(services);
        }
    }
}
