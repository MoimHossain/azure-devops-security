

using CommandLine;
using Kdoctl;
using Kdoctl.CliOptions;
using Kdoctl.CliServices.AzDoServices.LowLevels;
using Kdoctl.CliServices.K8sServices;
using Kdoctl.CliServices.Supports;
using Kdoctl.CliServices.Supports.Instrumentations;
using Kdoctl.Schema.CliServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

var parsedObject = Parser.Default.ParseArguments<
    ApplyOptions, 
    ExportOptions, 
    WorkItemMigrateOptions>(args);

if (!parsedObject.Errors.Any() && parsedObject.Value is OptionBase baseOpts)
{
    var consoleHost = new HostBuilder()
                    .ConfigureServices((hostContext, services) =>
                    {
                        baseOpts.Sanitize();
                        services.AddSingleton<OptionBase>(baseOpts);
                        services.AddTelemetryServices(baseOpts);
                        services.AddServicesFromClientLib(baseOpts);
                        services.AddHttpClients(baseOpts);
                        services.AddServices();                        
                        services.AddSingleton<PatternMatchAssistant>();
                        services.AddTransient<CliRunner>();
                    })
                    .UseConsoleLifetime()
                                .Build();

    using var serviceScope = consoleHost.Services.CreateScope();
    var runner = serviceScope.ServiceProvider.GetRequiredService<CliRunner>();

    return parsedObject.MapResult(
        (ApplyOptions applyOpts) => { return runner.RunApplyVerb(applyOpts); },
        (ExportOptions exportOpts) => { return runner.RunExportVerb(exportOpts); },
        (WorkItemMigrateOptions wiMigrateOpts) => { return runner.RunWorkItemMigrateVerb(wiMigrateOpts); },
        errs => 1);
}
else
{
    ConsoleLogger.NewLineMessage("Arguments are invalid");
    if(parsedObject.Errors.Any())
    {
        foreach(var item in parsedObject.Errors)
        {
            ConsoleLogger.NewLineMessage(item.Tag.ToString());
        }
    }
}
return -1;



