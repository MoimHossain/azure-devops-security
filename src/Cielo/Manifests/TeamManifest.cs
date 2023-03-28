

using Cielo.Azdo.Dtos;
using Cielo.Manifests.Common;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using static Cielo.Manifests.GroupManifest;

namespace Cielo.Manifests
{
    public class TeamManifest : ManifestBase
    {
        public List<UserReference> Admins { get; set; }
        public GroupMembershipManifest Membership { get; set; }
        public VstsTeamConfig Config { get; set; }

        public class VstsTeamConfig
        {
            public string DefaultPath { get; set; }
            public List<TeamConfigAreaPathsManifest> AreaPaths { get; set; }
        }

        public class TeamConfigAreaPathsManifest
        {
            public string Path { get; set; }

            public bool IncludeSubAreas { get; set; }
        }
    }
}
