﻿
using Kdoctl.CliOptions;
using Kdoctl.CliServices;
using Kdoctl.CliServices.Tasks;
using Kdoctl.Schema;
using System;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Kdoctl
{
    public class CliRunner
    {
        private readonly IDeserializer deserializer;
        private readonly IServiceProvider services;

        public CliRunner(IServiceProvider services)
        {
            deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
            this.services = services;
        }

        public int RunWorkItemMigrateVerb(WorkItemMigrateOptions opts)
        {
            Console.ResetColor();
            new WorkItemMigrationTask(services, opts).ExecuteAsync().Wait();
            Console.ResetColor();
            return 0;
        }

        public int RunExportVerb(ExportOptions opts)
        {
            Console.ResetColor();
            new ExportTask(services, opts).ExecuteAsync().Wait();
            Console.ResetColor();
            return 0;
        }

        public int RunApplyVerb(ApplyOptions opts)
        {
            Console.ResetColor();
            if (!string.IsNullOrWhiteSpace(opts.Directory))
            {
                if (Directory.Exists(opts.Directory))
                {
                    foreach(var spec in Directory.GetFiles(opts.Directory, "*.y*ml")
                        .Select(file => GetMetadata(opts, services, deserializer, file))
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
                    var spec = GetMetadata(opts, services, deserializer, file);
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
            ApplyOptions opts, IServiceProvider services, IDeserializer deserializer, string file)
        {
            if (File.Exists(file))
            {
                var payload = File.ReadAllText(file);
                var baseSchema = deserializer.Deserialize<BaseSchema>(payload);
                var service = MappedApiServiceAttribute.GetApiServiceInstance(baseSchema.Kind, services);
                return new Tuple<BaseSchema, string, TaskBase, string>(baseSchema, file, service, payload);
            }
            throw new InvalidOperationException("Manifest file couldn't be found or loaded.");
        }
    }
}
