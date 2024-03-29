﻿using Kdoctl.CliServices.AzDoServices;
using Kdoctl.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices
{
    public partial class StateSynchronizationTask : TaskBase
    {
        public StateSynchronizationTask(IServiceProvider services) : base(services)
        {
        }

        protected async override Task ExecuteCoreAsync(BaseSchema baseSchema, string manifestContent, string filePath)
        {
            var manifest = Deserialize<ProjectManifest>(manifestContent);            
            var projectService = GetProjectService();
            var repoService = GetRepositoryService();


            using var op = Insights.BeginOperation($"Validating Manifest file [{filePath}]...", "Validation");
            if (manifest.Validate())
            {   
                var outcome = await EnsureProjectExistsAsync(manifest, projectService);

                await ProcessPermissionsAsync(manifest,  projectService, outcome);
                await EnsureTeamProvisionedAsync(manifest,  projectService, outcome);
                await EnsureRepositoriesExistsAsync(manifest,  repoService, outcome.Item1, outcome.Item2);
                var seOutcome = await EnsureServiceEndpointExistsAsync(manifest, projectService,  outcome);
                await EnsureEnvironmentExistsAsync(manifest,  outcome.Item1, seOutcome);
                await EnsureBuildFoldersAsync(manifest,  outcome.Item1);
                await EnsureReleaseFoldersAsync(manifest,  outcome.Item1);
            }
            else
            {
                op.EndWithFailure("Invalid manifest file!");
            }
        }

        protected override Task ExecuteCoreAsync()
        {
            throw new NotImplementedException();
        }
    }    
}
