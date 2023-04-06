
using Cielo.Manifests.Common;
using Cielo.ResourceManagers.Abstract;
using Cielo.ResourceManagers.ResourceStates;
using Cielo.ResourceManagers.Supports;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Cielo.CliSupports
{
    public class CommandProcessor
    {
        private readonly ApplyOption applyOption;
        private readonly IDeserializer deserializer;
        private readonly ResourceProcessingContext context;
        private readonly IServiceProvider serviceProvider;

        public CommandProcessor(
            ApplyOption applyOption,
            IDeserializer deserializer,
            ResourceProcessingContext context,
            IServiceProvider serviceProvider)
        {
            this.applyOption = applyOption ?? throw new ArgumentNullException(nameof(applyOption));
            this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            this.context = context;
            this.serviceProvider = serviceProvider;
        }

        public async Task ProcessAsync()
        {
            var manifestFiles = GetManifestFiles();
            var resourceManagers = ReadMetadataSignatures(manifestFiles);
            var reports = new List<ReportManager>();

            foreach (var resourceManager in resourceManagers)
            {
                var (beforeState, afterState) = await resourceManager.PlanAsync();

                var reportManager = new ReportManager(resourceManager, beforeState, afterState);
                reportManager.Report();
                reports.Add(reportManager);
            }
            await Task.CompletedTask;
        }


        private IEnumerable<ResourceManagerBase> ReadMetadataSignatures(IEnumerable<string> files)
        {
            var unsortedCollection = new List<Tuple<ManifestBase, string>>();
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    var payload = File.ReadAllText(file);
                    var manifestBase = deserializer.Deserialize<ManifestBase>(payload);

                    unsortedCollection.Add(new Tuple<ManifestBase, string>(manifestBase, payload));
                }
            }

            foreach (var tuple in unsortedCollection.OrderBy(s => s.Item1.Kind))
            {
                var manager = ManifestResourceMapAttribute.GetManifestFromYaml(serviceProvider, tuple.Item1, tuple.Item2);
                if (manager != null)
                {
                    yield return manager;
                }
            }
        }

        private IEnumerable<string> GetManifestFiles()
        {
            var files = new List<string>();
            if (!string.IsNullOrWhiteSpace(applyOption.Directory))
            {
                if (Directory.Exists(applyOption.Directory))
                {
                    files.AddRange(Directory.GetFiles(applyOption.Directory, "*.y*ml",
                        new EnumerationOptions()
                        {
                            RecurseSubdirectories = true,
                            MaxRecursionDepth = 10
                        }));
                }
            }
            if (applyOption.ManifestFiles != null && applyOption.ManifestFiles.Any())
            {
                files.AddRange(applyOption.ManifestFiles);
            }
            return files;
        }
    }
}
