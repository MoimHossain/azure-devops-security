using Cielo.ResourceManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Manifests.Common
{
    public enum ManifestKind
    {
        [ManifestResourceMapAttribute(typeof(ProjectResourceManager))] Project = 0,
        [ManifestResourceMapAttribute(typeof(GroupResourceManager))] Group = 1,
        [ManifestResourceMapAttribute(typeof(AreaPathSecurityManager))] AreaPathSecurity = 2,
        [ManifestResourceMapAttribute(typeof(TeamResourceManager))] Team = 3,
        [ManifestResourceMapAttribute(typeof(RepositorySecurityManager))] RepositorySecurity = 4,
        [ManifestResourceMapAttribute(typeof(PipelineFolderSecurityManager))] PipelineFolderSecurity = 5,
        [ManifestResourceMapAttribute(typeof(ReleaseFolderSecurityManager))] ReleaseFolderSecurity = 6,
        
    }
}
