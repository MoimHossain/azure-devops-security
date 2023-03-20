
using Cielo.Manifests.Common;
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

        public CommandProcessor(ApplyOption applyOption, IDeserializer deserializer)
        {
            this.applyOption = applyOption ?? throw new ArgumentNullException(nameof(applyOption));
            this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public async Task ProcessAsync()
        {
            var manifestFiles = GetManifestFiles();
            var manifests = ReadMetadataSignatures(manifestFiles);

            foreach(var  manifest in manifests)
            {

            }

            await Task.CompletedTask;
        }


        private IEnumerable<(ManifestBase, ManifestKind)> ReadMetadataSignatures(IEnumerable<string> files)
        {
            foreach(var file in files)
            {
                if (File.Exists(file))
                {
                    var payload = File.ReadAllText(file);
                    var manifestBase = deserializer.Deserialize<ManifestBase>(payload);
                    yield return (manifestBase, manifestBase.Kind);
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
