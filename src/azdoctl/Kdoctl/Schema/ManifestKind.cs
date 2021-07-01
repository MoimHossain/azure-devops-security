using Kdoctl.Schema.CliServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.Schema
{
    public enum ManifestKind
    {
        [MappedApiServiceAttribute(typeof(ProjectApiService))]
        Project,
        Repository,
        Group,
        BuildFolder,
        ReleaseFolder,
        AreaPath,
        IterationPath
    }
}
