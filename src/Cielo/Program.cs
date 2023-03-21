
using Cielo.CliSupports;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Cielo.Manifests.Supports;

var parsedObject = Parser.Default.ParseArguments<ApplyOption>(args);
if (!parsedObject.Errors.Any() && parsedObject.Value is not null)
{
    var option = parsedObject.Value as ApplyOption;
    var consoleHost = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
                    {
                        option.Sanitize();
                        services.AddSingleton(option);

                        IDeserializer deserializer = (Deserializer)new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .IgnoreUnmatchedProperties()
                            .Build();
                        ISerializer serializer = new SerializerBuilder()
                           .WithNamingConvention(CamelCaseNamingConvention.Instance)
                           .WithEmissionPhaseObjectGraphVisitor(args => new YamlIEnumerableSkipEmptyObjectGraphVisitor(args.InnerVisitor))
                           .Build();

                        services.AddSingleton(serializer);
                        services.AddSingleton(deserializer);

                        services.AddSingleton<CommandProcessor>();
                    })
                    .UseConsoleLifetime()
                                .Build();
    using var serviceScope = consoleHost.Services.CreateScope();
    var cmdProcessor = serviceScope.ServiceProvider.GetRequiredService<CommandProcessor>();
    await cmdProcessor.ProcessAsync();
    return 0;
}
else
{

    if (parsedObject.Errors.Any())
    {
        foreach (var item in parsedObject.Errors)
        {

        }
    }
}
return -1;