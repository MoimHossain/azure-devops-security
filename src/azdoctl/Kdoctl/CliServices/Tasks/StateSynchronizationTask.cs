using Kdoctl.CliServices.AzDoServices;
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
        public StateSynchronizationTask(string orgUri, string pat) : base(orgUri, pat)
        {
        }

        protected async override Task ExecuteCoreAsync(BaseSchema baseSchema, string manifestContent, string filePath)
        {
            var manifest = Deserialize<ProjectManifest>(manifestContent);
            var factory = base.Factory;
            var projectService = factory.GetProjectService();
            var repoService = factory.GetRepositoryService();


            Logger.StatusBegin($"Validating Manifest file [{filePath}]...");
            if (manifest.Validate())
            {
                Logger.StatusEndSuccess("Succeed");
                var outcome = await EnsureProjectExistsAsync(manifest, projectService);

                await ProcessPermissionsAsync(manifest, factory, projectService, outcome);
                await EnsureTeamProvisionedAsync(manifest, factory, projectService, outcome);
                await EnsureRepositoriesExistsAsync(manifest, factory, repoService, outcome.Item1, outcome.Item2);
                var seOutcome = await EnsureServiceEndpointExistsAsync(manifest, projectService, factory, outcome);
                await EnsureEnvironmentExistsAsync(manifest, factory, outcome.Item1, seOutcome);
                await EnsureBuildFoldersAsync(manifest, factory, outcome.Item1);
                await EnsureReleaseFoldersAsync(manifest, factory, outcome.Item1);
            }
            else
            {
                Logger.StatusEndFailed("Failed");
            }
        }

        protected override Task ExecuteCoreAsync()
        {
            throw new NotImplementedException();
        }
    }    
}
