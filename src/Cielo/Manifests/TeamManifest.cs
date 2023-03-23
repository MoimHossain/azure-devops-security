using Cielo.Manifests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Manifests
{
    public class TeamManifest : ManifestBase
    {
        public List<UserReference> Admins { get; set; }

        public class UserReference
        {
            public string Name { get; set; }
            public string Principal { get; set; }
        }
    }
}
