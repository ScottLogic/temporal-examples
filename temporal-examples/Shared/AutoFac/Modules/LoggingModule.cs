using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.OpenTelemetry;
using Serilog.Enrichers.Span;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Json;

namespace Shared.AutoFac.Modules
{
    /// <summary>
    /// Autofac module for configuring logging services
    /// </summary>
    public class LoggingModule : Module
    {
        private readonly IConfiguration _configuration;

        public LoggingModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var loggerConfig = new LoggerConfiguration()
                .Enrich.WithSpan()
                .Enrich.WithOpenTelemetryTraceId()
                .Enrich.WithOpenTelemetrySpanId()
                .Enrich.WithProperty(
                    "Environment",
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
                )
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .WriteTo.File(
                    new JsonFormatter(renderMessage: true),
                    "logs/log-.json",
                    rollingInterval: RollingInterval.Day,
                    shared: true
                )
                .WriteTo.OpenTelemetry(options =>
                {
                    options.Endpoint = "http://host.docker.internal:4317"; // Use your OTLP endpoint
                    options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.Grpc; // or .HttpProtobuf
                    options.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = "temporal-worker",
                    };
                });

            var logger = loggerConfig.CreateLogger();

            Log.Logger = logger;

            builder.RegisterInstance(logger).As<Serilog.ILogger>().SingleInstance();

            builder
                .Register<ILoggerFactory>(c => new SerilogLoggerFactory(logger, true))
                .As<ILoggerFactory>()
                .SingleInstance();
        }
    }
}
