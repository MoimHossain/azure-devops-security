﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.Schema
{
    public class ProjectManifest : BaseSchema
    {
        public ProcessTemplateSchema Template { get; set; }
        public List<RepositoryManifest> Repositories { get; set; }
        public List<EnvironmentManifest> Environments { get; set; }
        public List<PipelineFolder> BuildFolders { get; set; }
        public List<PipelineFolder> ReleaseFolders { get; set; }
        public List<TeamSchemaManifest> Teams { get; set; }

        public List<ServiceEndpointManifest> ServiceEndpoints { get; set; }
        protected override bool OnValidateCore()
        {
            if (Template == null || string.IsNullOrWhiteSpace(Template.Name))
            {
                return false;
            }

            return base.OnValidateCore();
        }
    }

    public class ProcessTemplateSchema
    {
        public string Name { get; set; }
        public string SourceControlType { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class PipelineFolder
    {
        public string Path { get; set; }

        public List<PermissionManifest> Permissions { get; set; }
    }
}
