
using System.Threading.Tasks;

namespace Kdoctl.CliServices.Controllers.Abstract
{
    public abstract class ControllerBase<TResourceManifest>
    {
        private readonly TResourceManifest resourceManifest;

        protected ControllerBase(TResourceManifest resourceManifest)
        {
            this.resourceManifest = resourceManifest;
        }

        public abstract Task<TResourceManifest> GetAsync();

        public abstract Task DeleteAsync();

        public abstract Task CreateAsync();

        public abstract Task UpdateAsync();
    }
}
