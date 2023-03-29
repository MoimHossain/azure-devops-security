using Cielo.Manifests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Cielo.Azdo.PermissionEnums;
using static Cielo.Manifests.GroupManifest;

namespace Cielo.Manifests
{
    public class IterationPathSecurityManifest : ManifestBase
    {
        public List<IterationPathSecurityPermissionManifest> Permissions { get; set; }

        public class IterationPathSecurityPermissionManifest
        {
            public List<string> Paths { get; set; }
            public List<Iteration> Allowed { get; set; }

            public List<UserReference> Users { get; set; }
            public List<GroupReference> Groups { get; set; }
        }
    }
}
