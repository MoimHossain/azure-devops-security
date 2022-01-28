

using Kdoctl.CliServices.AzDoServices;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Constants;
using Kdoctl.CliServices.Supports;
using Kdoctl.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices
{
    public partial class StateSynchronizationTask
    {
        protected async Task EnsureRepositoriesExistsAsync(
            ProjectManifest manifest, 
            RepositoryService repoService,
            Kdoctl.CliServices.AzDoServices.Dtos.Project project,
            bool projectWasAbsent)
        {
            if (project != null && manifest.Repositories != null && manifest.Repositories.Any())
            {
                foreach (var repo in manifest.Repositories)
                {
                    if (!string.IsNullOrWhiteSpace(repo.Name))
                    {
                        var reposCollection = await repoService.GetRepositoryListAsync(project.Id);
                        var repository = reposCollection
                            .FirstOrDefault(r => r.Name.Equals(repo.Name, StringComparison.OrdinalIgnoreCase));

                        if (repository == null)
                        {
                            using var op = Logger.Begin($"Creating Repository {repo.Name}...", "Repository");
                            await ExecutionSupports.Retry(async () =>
                            {
                                repository = await repoService.CreateAsync(project.Id, repo.Name);
                            },
                                exception => { Logger.Error(exception); });
                            op.EndWithSuccess("Succeed");
                        }

                        using var opPermissions = Logger.Begin($"Setting up permissions for repository {repo.Name}...", "RepoPermissions");
                        await EnsureRepositoryPermissionsAsync( project, repo, repository);                        
                    }
                }
                await DeleteDefaultRepoAsync(repoService, project, projectWasAbsent);
            }
        }

        protected async Task EnsureRepositoryPermissionsAsync(
            Kdoctl.CliServices.AzDoServices.Dtos.Project project,
            RepositoryManifest repo, Microsoft.TeamFoundation.SourceControl.WebApi.GitRepository repository)
        {
            if (repository != null && repo.Permissions != null && repo.Permissions.Any())
            {
                var secService = GetSecurityNamespaceService();
                var gitNamespace = await secService.GetNamespaceAsync(SecurityNamespaceConstants.Git_Repositories);
                var gitSecurityNamespaceId = gitNamespace.NamespaceId;
                var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();
                await CreateAclsAsync(typeof(GitRepositories), repo.Permissions, aclDictioanry);

                if (aclDictioanry.Count > 0)
                {
                    var repositorySecurityToken = $"repoV2/{project.Id}/{repository.Id}";
                    var aclService = GetAclListService();
                    await aclService.SetAclsAsync(gitSecurityNamespaceId, repositorySecurityToken, aclDictioanry, false);
                }
            }
        }


        protected async Task DeleteDefaultRepoAsync(
            RepositoryService repoService,
            Kdoctl.CliServices.AzDoServices.Dtos.Project project, bool projectWasAbsent)
        {
            if (projectWasAbsent)
            {
                // when new projects are created, there's a defaul repository
                // let's remove that
                var allRepos = await repoService.GetRepositoryListAsync(project.Id);
                if (allRepos != null && allRepos.Count > 0)
                {
                    await repoService.DeleteRepositoryAsync(project.Id, allRepos.First().Id);
                }
            }
        }
    }
}
