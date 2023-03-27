
using Cielo.Manifests.Common;
using static Cielo.Azdo.PermissionEnums;
using static Cielo.Manifests.GroupManifest;

namespace Cielo.Manifests
{
    public class RepositorySecurityManifest : ManifestBase
    {
        public List<RepositoryPermissionManifest> Permissions { get; set; }

        public class RepositoryPermissionManifest
        {
            public List<string> Names { get; set; }
            public List<GitRepositories> Allowed { get; set; }

            public List<UserReference> Users { get; set; }
            public List<GroupReference> Groups { get; set; }
        }
    }
}
