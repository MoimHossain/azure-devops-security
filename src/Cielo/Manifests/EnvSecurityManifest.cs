

using Cielo.Manifests.Common;
using static Cielo.Manifests.GroupManifest;

namespace Cielo.Manifests
{
    public class EnvSecurityManifest : ManifestBase
    {
        public List<EnvPermissionManifest> Permissions { get; set; }

        public class EnvPermissionManifest
        {
            public List<string> Names { get; set; }
            public List<AzdoEnvRoles> Roles { get; set; }

            public List<UserReference> Users { get; set; }
            public List<GroupReference> Groups { get; set; }

            public bool ApplyToRoot { get; set; }
        }

        public enum AzdoEnvRoles
        {
            Creator,
            User,
            Reader,
            Administrator
        }
    }
}
