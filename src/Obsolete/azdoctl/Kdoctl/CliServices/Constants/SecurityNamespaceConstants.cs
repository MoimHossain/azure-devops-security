

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kdoctl.CliServices.Constants
{
    public enum SecurityNamespaceConstants
    {
        [Description("CSS")]
        Classifications,
        [Description("Git Repositories")]
        Git_Repositories,
        [Description("ReleaseManagement")]
        ReleaseManagement,
        [Description("Build")]
        Build,
        [Description("Identity")]
        Identity
    }
}
