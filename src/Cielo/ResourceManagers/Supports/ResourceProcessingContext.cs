using Cielo.Azdo.Dtos;
using Cielo.Manifests;

namespace Cielo.ResourceManagers.Supports
{
    public class ResourceProcessingContext
    {
        public ResourceProcessingContext() { }

        public ProjectManifest CurrentProjectManifest { get; set; }

        public Project CurrentProject { get; set; }
    }
}
