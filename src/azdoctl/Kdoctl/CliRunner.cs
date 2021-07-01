using Kdoctl.CliOptions;
using Kdoctl.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Kdoctl
{
    public class CliRunner
    {
        public int RunApplyVerb(ApplyOptions opts)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            foreach (var file in opts.ManifestFiles)
            {
                if (File.Exists(file))
                {
                    var payload = File.ReadAllText(file);
                    var baseSchema = deserializer.Deserialize<BaseSchema>(payload);
                    MappedApiServiceAttribute
                        .GetApiServiceInstance(baseSchema.Kind,
                            opts.OrganizationURL,
                            opts.PAT)
                        .ExecuteAsync(baseSchema, payload).Wait();
                }
            }
            Console.ResetColor();
            return 0;
        }
    }
}
