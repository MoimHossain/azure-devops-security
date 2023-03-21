using Cielo.Manifests.Common;
using Cielo.ResourceManagers.ResourceStates;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Cielo.ResourceManagers.Abstract
{
    public abstract class ResourceManagerBase
    {
        private readonly IServiceProvider serviceProvider;
        private readonly string rawManifest;
        private ManifestBase? fullManifest;

        protected ResourceManagerBase(IServiceProvider serviceProvider, string rawManifest)
        {
            this.serviceProvider = serviceProvider;
            this.rawManifest = rawManifest;
            this.fullManifest = DeserializeCore();
        }

        public async Task PlanAsync()
        {
            if(fullManifest != null)
            {
                var state = await GetAsync();
            }
           
            await Task.CompletedTask;
        }

        protected virtual ManifestBase? DeserializeCore()
        {
            var deserializer = this.serviceProvider.GetRequiredService<IDeserializer>();
            var item = (ManifestBase?)deserializer.Deserialize(this.rawManifest, GetResourceType());
            return item;
        }

        protected abstract Type GetResourceType();

        protected ManifestBase? Manifest { get { return this.fullManifest; } }

        protected abstract Task<ResourceState> GetAsync();
    }
}
