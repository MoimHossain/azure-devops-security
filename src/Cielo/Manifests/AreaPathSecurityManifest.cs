

using Cielo.Manifests.Common;
using static Cielo.Azdo.PermissionEnums;
using static Cielo.Manifests.GroupManifest;

namespace Cielo.Manifests
{
    public class AreaPathSecurityManifest : ManifestBase
    {
        public List<AreaPathSecurityPermissionManifest> Permissions { get; set; }

        public class AreaPathSecurityPermissionManifest
        {
            public List<string> Paths { get; set; }
            public List<CSS> Allowed { get; set; }

            public List<UserReference> Users { get; set; }
            public List<GroupReference> Groups { get; set; }
        }
    }
}
