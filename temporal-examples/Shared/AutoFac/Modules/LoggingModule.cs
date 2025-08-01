using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace Shared.AutoFac.Modules
{
    /// <summary>
    /// Autofac module for configuring logging services
    /// </summary>
    public class LoggingModule : Module
    {
        private readonly IConfiguration _configuration;

        public LoggingModule(
            IConfiguration configuration
        )
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var loggerConfig = new LoggerConfiguration()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

            var logger = loggerConfig.CreateLogger();

            Log.Logger = logger;

            builder.RegisterInstance(logger).As<Serilog.ILogger>().SingleInstance();

            builder.Register<ILoggerFactory>(c => new SerilogLoggerFactory(logger, true))
                .As<ILoggerFactory>()
                .SingleInstance();
        }
    }
}
