using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kdoctl.CliServices.AzDoServices.PipelineEnvironmentService;

namespace Kdoctl.Schema
{
    public class EnvironmentManifest
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<EnvironmentPermissionManifest> Permissions { get; set; }

        public string ServiceEndpointReference { get; set; }
    }

    public class EnvironmentPermissionManifest
    {
        public string Group { get; set; }
        public Guid Id { get; set; }
        public string Origin { get; set; }
        public List<PipelineEnvironmentPermissions> Roles { get; set; }
    }
}
