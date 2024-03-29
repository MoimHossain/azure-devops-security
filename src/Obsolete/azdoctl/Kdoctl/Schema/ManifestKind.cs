﻿using Kdoctl.CliServices;
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
        [MappedApiServiceAttribute(typeof(StateSynchronizationTask))]
        Project,
        [MappedApiServiceAttribute(typeof(StateSynchronizationTask))]
        Repository,
        [MappedApiServiceAttribute(typeof(StateSynchronizationTask))]
        Permission,
        [MappedApiServiceAttribute(typeof(StateSynchronizationTask))]
        Team
    }
}
