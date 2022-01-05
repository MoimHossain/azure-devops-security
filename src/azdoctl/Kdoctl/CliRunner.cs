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

            if (!string.IsNullOrWhiteSpace(opts.Directory))
            {
                if (Directory.Exists(opts.Directory))
                {
                    foreach (var filePath in Directory.GetFiles(opts.Directory, "*.y*ml"))
                    {
                        ProcessSingleFileAsync(opts, deserializer, filePath);
                    }
                }
            }
            

            if(opts.ManifestFiles != null && opts.ManifestFiles.Any())
            {
                foreach (var file in opts.ManifestFiles)
                {
                    ProcessSingleFileAsync(opts, deserializer, file);
                }
            }
            Console.ResetColor();
            return 0;
        }

        private static void ProcessSingleFileAsync(
            ApplyOptions opts, IDeserializer deserializer, string file)
        {
            if (File.Exists(file))
            {
                var payload = File.ReadAllText(file);
                var baseSchema = deserializer.Deserialize<BaseSchema>(payload);
                MappedApiServiceAttribute
                    .GetApiServiceInstance(baseSchema.Kind,
                        opts.OrganizationURL,
                        opts.PAT)
                    .ExecuteAsync(baseSchema, payload, file).Wait();
            }
        }
    }
}
