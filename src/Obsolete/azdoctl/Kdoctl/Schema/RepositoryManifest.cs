using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.Schema
{
    public class RepositoryManifest
    {
        public string Name { get; set; }

        public List<PermissionManifest> Permissions { get; set; }
    }

    public class PermissionManifest
    {
        public string Group { get; set; }
        public Guid Id { get; set; }
        public string Origin { get; set; }
        public List<string> Allowed { get; set; }
    }
}
