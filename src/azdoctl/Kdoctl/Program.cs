

using CommandLine;
using Kdoctl;
using Kdoctl.CliOptions;
using Kdoctl.CliServices.AzDoServices.LowLevels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;


var parsedObject = Parser.Default.ParseArguments<ApplyOptions, ExportOptions>(args);

if (parsedObject.Errors.Count() <= 0 && parsedObject.Value is OptionBase baseOpts)
{    
    #pragma warning disable CA1416 // Validate platform compatibility
    var consoleHost = new HostBuilder()
                    .ConfigureServices((hostContext, services) =>
                    {
                        baseOpts = OptionBase.Sanitize(baseOpts);
                        services.AddServicesFromClientLib(baseOpts);
                        services.AddHttpClients(baseOpts);
                        services.AddServices();
                        services.AddTransient<CliRunner>();
                    })
                    .UseConsoleLifetime()
                #pragma warning restore CA1416 // Validate platform compatibility
                .Build();


    using (var serviceScope = consoleHost.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;
        var runner = services.GetRequiredService<CliRunner>();

        return Parser.Default.ParseArguments<ApplyOptions, ExportOptions>(args)
            .MapResult(
            (ApplyOptions applyOpts) => { return runner.RunApplyVerb(OptionBase.Sanitize(applyOpts)); },
            (ExportOptions exportOpts) => { return runner.RunExportVerb(OptionBase.Sanitize(exportOpts)); },
            errs => 1);
    }
}
return -1;



