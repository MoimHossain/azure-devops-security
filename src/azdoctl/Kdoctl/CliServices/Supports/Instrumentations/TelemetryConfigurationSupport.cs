

using Kdoctl.CliOptions;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.Extensions.DependencyInjection;

namespace Kdoctl.CliServices.Supports.Instrumentations
{
    public static class TelemetryConfigurationSupport
    {
        public static void AddTelemetryServices(this IServiceCollection services, OptionBase opts)
        {
            services.AddSingleton(new TelemetryConfiguration
            {
                ConnectionString = opts.AppInsightConnectionString,
                TelemetryChannel = new ServerTelemetryChannel()
            });
            var configuration = new TelemetryConfiguration
            {
                ConnectionString = opts.AppInsightConnectionString,
                TelemetryChannel = new ServerTelemetryChannel()
            };
            configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

            services.AddSingleton(configuration);
            services.AddSingleton<InstrumentationClient>();
        }
    }
}
