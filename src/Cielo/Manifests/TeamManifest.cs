using Cielo.Manifests.Common;
using static Cielo.Manifests.GroupManifest;

namespace Cielo.Manifests
{
    public class TeamManifest : ManifestBase
    {
        public List<UserReference> Admins { get; set; }
        public GroupMembershipManifest Membership { get; set; }
    }
}
