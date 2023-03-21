
using Cielo.Manifests.Common;
using Cielo.ResourceManagers.Abstract;
using System;
using System.Collections.Generic;
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
        private readonly IServiceProvider serviceProvider;

        public CommandProcessor(ApplyOption applyOption, IDeserializer deserializer, IServiceProvider serviceProvider)
        {
            this.applyOption = applyOption ?? throw new ArgumentNullException(nameof(applyOption));
            this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            this.serviceProvider = serviceProvider;
        }

        public async Task ProcessAsync()
        {
            var manifestFiles = GetManifestFiles();
            var resourceManagers = ReadMetadataSignatures(manifestFiles);

            foreach (var resourceManager in resourceManagers)
            {
                await resourceManager.PlanAsync();
            }

            await Task.CompletedTask;
        }


        private IEnumerable<ResourceManagerBase> ReadMetadataSignatures(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    var payload = File.ReadAllText(file);
                    var manifestBase = deserializer.Deserialize<ManifestBase>(payload);
                    var manager = ManifestResourceMapAttribute.GetManifestFromYaml(serviceProvider, manifestBase, payload);
                    if (manager != null)
                    {
                        yield return manager;
                    }
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
                    files.AddRange(Directory.GetFiles(applyOption.Directory, "*.y*ml"));
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
