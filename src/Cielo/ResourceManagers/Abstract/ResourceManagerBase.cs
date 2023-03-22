
using Cielo.Manifests.Common;
using Cielo.ResourceManagers.ResourceStates;
using Cielo.ResourceManagers.Supports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using YamlDotNet.Serialization;

namespace Cielo.ResourceManagers.Abstract
{
    public abstract class ResourceManagerBase
    {
        private readonly ResourceProcessingContext context;
        private readonly IServiceProvider serviceProvider;
        private readonly string rawManifest;        
        private ManifestBase? fullManifest;

        protected ResourceManagerBase(IServiceProvider serviceProvider, string rawManifest)
        {
            this.serviceProvider = serviceProvider;
            this.rawManifest = rawManifest;
            this.fullManifest = DeserializeCore();
            this.context = serviceProvider.GetRequiredService<ResourceProcessingContext>();

            this.SetContextData();
        }

        

        public async Task<(ResourceState, ResourceState?)> PlanAsync()
        {
            var beforeState = await GetAsync();
            var afterState = default(ResourceState);

            if(beforeState.Exists)
            {
                afterState = await UpdateAsync();
            }
            else
            {
                afterState = await CreateAsync();
            }

            return (beforeState, afterState);
        }

      

        protected virtual ManifestBase? DeserializeCore()
        {
            var deserializer = this.serviceProvider.GetRequiredService<IDeserializer>();
            var item = (ManifestBase?)deserializer.Deserialize(this.rawManifest, GetResourceType());
            return item;
        }

        protected abstract Type GetResourceType();

        public ManifestBase? Manifest { get { return this.fullManifest; } }

        protected virtual void SetContextData() { }

        protected ResourceProcessingContext Context { get { return this.context; } }

        protected abstract Task<ResourceState> GetAsync();
        protected abstract Task<ResourceState> CreateAsync();
        protected abstract Task<ResourceState?> UpdateAsync();
    }
}
