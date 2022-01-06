using Kdoctl.CliOptions;
using Kdoctl.CliServices;
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
                    foreach(var spec in Directory.GetFiles(opts.Directory, "*.y*ml")
                        .Select(file => GetMetadata(opts, deserializer, file))
                        .OrderBy(metadata => metadata.Item1.Kind))
                    {
                        ProcessSpec(spec.Item3, spec.Item4, spec.Item1, spec.Item2);
                    }
                }
            }           

            if(opts.ManifestFiles != null && opts.ManifestFiles.Any())
            {
                foreach (var file in opts.ManifestFiles)
                {
                    var spec = GetMetadata(opts, deserializer, file);
                    ProcessSpec(spec.Item3, spec.Item4, spec.Item1, spec.Item2);
                }
            }
            Console.ResetColor();
            return 0;
        }      

        private static void ProcessSpec(
            TaskBase task, string payload, 
            BaseSchema metadata, string file)
        {
            task.ExecuteAsync(metadata, payload, file).Wait();
        }

        private static Tuple<BaseSchema, string, TaskBase, string> GetMetadata(
            ApplyOptions opts, IDeserializer deserializer, string file)
        {
            if (File.Exists(file))
            {
                var payload = File.ReadAllText(file);
                var baseSchema = deserializer.Deserialize<BaseSchema>(payload);
                var service = MappedApiServiceAttribute
                 .GetApiServiceInstance(baseSchema.Kind, opts.OrganizationURL, opts.PAT);
                return new Tuple<BaseSchema, string, TaskBase, string>(baseSchema, file, service, payload);
            }
            throw new InvalidOperationException("Manifest file couldn't be found or loaded.");
        }
    }
}
