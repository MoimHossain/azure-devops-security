
using Cielo.Azdo.Dtos;
using Cielo.Manifests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Cielo.Manifests.GroupManifest;

namespace Cielo.Manifests
{
    public class GroupManifest : ManifestBase
    {
        public GroupPropertiesManifest Properties { get; set; }
        public GroupMembershipManifest Membership { get; set; }

        public class GroupPropertiesManifest
        {
            public IdentityOrigin Origin { get; set; }
            public GroupScopeEnum Scope { get; set; }
            public Guid? AadObjectId { get; set; }
        }

        public enum GroupScopeEnum
        {
            Project,
            Organization
        }

        public class UserReference
        {
            public string Name { get; set; }
            public string Principal { get; set; }
        }
        public class GroupReference
        {
            public string Name { get; set; }
            public GroupScopeEnum Scope { get; set; }
            public IdentityOrigin Origin { get; set; }
        }

        public class GroupMembershipManifest
        {
            public List<UserReference> Users { get; set; }
            public List<GroupReference> Groups { get; set; }
        }
    }
}
