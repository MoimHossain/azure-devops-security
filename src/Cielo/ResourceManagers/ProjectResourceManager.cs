using Cielo.Manifests;
using Cielo.ResourceManagers.Abstract;
using Cielo.ResourceManagers.ResourceStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.ResourceManagers
{
    public class ProjectResourceManager : ResourceManagerBase
    {
        public ProjectResourceManager(IServiceProvider serviceProvider, string rawManifest) : base(serviceProvider, rawManifest) { }

        protected override Task<ResourceState> GetAsync()
        {
            throw new NotImplementedException();
        }

        protected override Type GetResourceType()
        {
            return typeof(ProjectManifest);
        }
    }
}
