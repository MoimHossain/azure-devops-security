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
        [ManifestResourceMapAttribute(typeof(ProjectResourceManager))]
        Project
    }
}
