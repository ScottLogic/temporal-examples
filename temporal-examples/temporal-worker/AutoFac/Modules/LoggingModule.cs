using System;
using Autofac;
using Microsoft.Extensions.Logging;

namespace TemporalWorker.AutoFac.Modules
{
    /// <summary>
    /// Autofac module for configuring logging services
    /// </summary>
    public class LoggingModule : Module
    {
        private readonly LogLevel _minimumLevel;

        /// <summary>
        /// Creates a new instance of the logging module with the specified minimum log level
        /// </summary>
        /// <param name="minimumLevel">Minimum log level to display</param>
        public LoggingModule(LogLevel minimumLevel = LogLevel.Information)
        {
            _minimumLevel = minimumLevel;
        }

        /// <summary>
        /// Configure the logging services with Autofac
        /// </summary>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();

            // Register factory for creating loggers
            builder
                .Register(c =>
                {
                    var factory = LoggerFactory.Create(loggingBuilder =>
                    {
                        loggingBuilder
                            .SetMinimumLevel(_minimumLevel)
                            .AddSimpleConsole(options =>
                            {
                                options.SingleLine = false;
                                options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                                options.UseUtcTimestamp = true;
                            });
                        // You can add additional providers here as needed
                    });
                    return factory;
                })
                .As<ILoggerFactory>()
                .SingleInstance();

            // Register a filter to respect minimum level
            builder
                .Register(c => new LoggerFilterOptions { MinLevel = _minimumLevel })
                .SingleInstance();
        }
    }
}
