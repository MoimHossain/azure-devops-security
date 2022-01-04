using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.Schema
{
    public class PermissionSchemaManifest
    {
        public string Name { get; set; }
        
        public TeamMembershipSchemaManifest Membership { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
