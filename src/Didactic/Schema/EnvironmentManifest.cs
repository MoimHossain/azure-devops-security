using System;
using System.Collections.Generic;
using System.Text;
using static Waddle.PipelineEnvironmentService;

namespace Didactic.Schema
{
    public class EnvironmentManifest
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<EnvironmentPermissionManifest> Permissions { get; set; }
    }

    public class EnvironmentPermissionManifest
    {
        public string Group { get; set; }
        public Guid Id { get; set; }
        public string Origin { get; set; }
        public List<PipelineEnvironmentPermissions> Roles { get; set; }
    }
}
