using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.Schema
{
    public class TeamSchemaManifest
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<AadObjectSchema> Admins { get; set; }

        public TeamMembershipSchemaManifest Membership { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class TeamMembershipSchemaManifest
    {
        public List<AadObjectSchema> Users { get; set; }
        public List<AadObjectSchema> Groups { get; set; }
    }

    public class AadObjectSchema
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
