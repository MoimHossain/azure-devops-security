﻿

using Didactic.Schema;
using Microsoft.TeamFoundation.Core.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Waddle.Constants;
using Waddle.Dtos;
using Waddle.Supports;

namespace Didactic.ApiServices
{
    public class ProjectApiService : BaseApiService
    {
        public ProjectApiService(string orgUri, string pat) : base(orgUri, pat)
        {
        }

        protected async override Task ExecuteCoreAsync(BaseSchema baseSchema, string manifestContent)
        {
            var manifest = Deserialize<ProjectManifest>(manifestContent);
            var factory = base.Factory;
            var projectService = factory.GetProjectService();
            var repoService = factory.GetRepositoryService();
            var templates = await projectService.ListProcessAsync();

            Logger.StatusBegin("Validating Manifest file...");
            if (manifest.Validate())
            {
                Logger.StatusEndSuccess("Succeed");

                Logger.StatusBegin("Creating project, reading Process templates...");
                var tempalte = templates.Value.FirstOrDefault(t => t.Name.Equals(manifest.Template.Name, StringComparison.InvariantCulture));
                if (tempalte == null)
                {
                    Logger.StatusEndFailed("Failed");
                    Logger.Error($"Process template {manifest.Template.Name} is not valid! Good example: Agile, CMMI, Basic, Scrum etc.");
                    return;
                }
                else
                {
                    Logger.StatusEndSuccess("Succeed");
                }

                var outcome = await EnsureProjectExistsAsync(manifest, projectService, tempalte);
                await EnsureTeamProvisionedAsync(manifest, factory, projectService, outcome);
                await EnsureRepositoriesExistsAsync(manifest, factory, repoService,
                    outcome.Item1, outcome.Item2);
                await EnsureEnvironmentExistsAsync(manifest, factory, outcome.Item1);
                await EnsureBuildFoldersAsync(manifest, factory, outcome.Item1);
                await EnsureReleaseFoldersAsync(manifest, factory, outcome.Item1);
            }
            else
            {
                Logger.StatusEndFailed("Failed");
            }
        }

        private async Task EnsureTeamProvisionedAsync(ProjectManifest manifest, Waddle.AdoConnectionFactory factory, Waddle.ProjectService projectService, Tuple<Waddle.Dtos.Project, bool> outcome)
        {
            var gService = factory.GetGroupService();
            var secService = factory.GetSecurityNamespaceService();
            var aclService = factory.GetAclListService();
            var allUsers = await gService.ListUsersAsync();
            foreach (var teamManifest in manifest.Teams)
            {
                var tc = await projectService.GetTeamsAsync();
                var eteam = tc.Value
                    .FirstOrDefault(tc => tc.Name.Equals(teamManifest.Name,
                    StringComparison.OrdinalIgnoreCase));

                if (eteam == null)
                {
                    Logger.StatusBegin($"Creating team [{teamManifest.Name}]...");
                    var team = await projectService.CreateTeamAsync(
                        new Microsoft.TeamFoundation.Core.WebApi.WebApiTeam
                        {
                            Name = teamManifest.Name,
                            Description = teamManifest.Description,
                            ProjectId = outcome.Item1.Id,
                            ProjectName = outcome.Item1.Name
                        },
                        outcome.Item1.Id);

                    while (eteam == null)
                    {
                        tc = await projectService.GetTeamsAsync();
                        eteam = tc.Value
                            .FirstOrDefault(tc => tc.Name.Equals(teamManifest.Name,
                            StringComparison.OrdinalIgnoreCase));
                    }
                    Logger.StatusEndSuccess("Succeed");
                }

                if (eteam != null && teamManifest.Membership != null
                    && (teamManifest.Membership.Groups != null && teamManifest.Membership.Groups.Any()))
                {
                    var teamGroup = await GetGroupByNameAsync(factory, IdentityOrigin.Vsts.ToString(), eteam.Name);
                    if (teamGroup != null)
                    {
                        foreach (var gp in teamManifest.Membership.Groups)
                        {
                            var groupObject = await GetGroupByNameAsync(factory, IdentityOrigin.Aad.ToString(), gp.Name, gp.Id);

                            if (groupObject != null)
                            {
                                await gService.AddMemberAsync(eteam.ProjectId, teamGroup.Descriptor, groupObject.Descriptor);
                            }
                        }

                        
                        foreach (var user in teamManifest.Membership.Users)
                        {
                            var userInfo = allUsers.Value.FirstOrDefault(u=> u.OriginId.Equals(user.Id));
                            if (userInfo != null )
                            {
                                await gService.AddMemberAsync(eteam.ProjectId, teamGroup.Descriptor, userInfo.Descriptor);
                            }
                        }
                    }                    
                }

                if (eteam != null &&
                    teamManifest.Admins != null &&
                    teamManifest.Admins.Any())
                {
                    var token = $"{eteam.ProjectId}\\{eteam.Id}";
                    var releaseNamespace = await secService.GetNamespaceAsync(SecurityNamespaceConstants.Identity);
                    var secNamespaceId = releaseNamespace.NamespaceId;
                    var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();

                    foreach (var adminUserName in teamManifest.Admins)
                    {
                        var matches = await gService.GetLegacyIdentitiesByNameAsync(adminUserName.Name);
                        if (matches != null && matches.Count > 0)
                        {
                            var adminUserInfo = matches.Value.First();
                            aclDictioanry.Add(adminUserInfo.Descriptor, new VstsAcesDictionaryEntry
                            {
                                Allow = 31,
                                Deny = 0,
                                Descriptor = adminUserInfo.Descriptor
                            });
                        }
                    }
                    await aclService.SetAclsAsync(secNamespaceId, token, aclDictioanry, false);
                }
            }
        }

        private static async Task DeleteDefaultRepoAsync(
            Waddle.RepositoryService repoService, 
            Waddle.Dtos.Project project, bool projectWasAbsent)
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

        private async Task EnsureReleaseFoldersAsync(ProjectManifest manifest,
          Waddle.AdoConnectionFactory factory, Waddle.Dtos.Project project)
        {
            if (manifest.ReleaseFolders != null && manifest.ReleaseFolders.Any())
            {
                var releaseService = factory.GetReleaseService();
                var releasePaths = await releaseService.ListFoldersAsync(project.Id);
                foreach (var rp in manifest.ReleaseFolders)
                {
                    var existingItem = releasePaths.Value
                        .FirstOrDefault(p => p.Path.Replace("\\", "/").Equals(rp.Path, StringComparison.OrdinalIgnoreCase));

                    if (existingItem == null)
                    {
                        existingItem = await releaseService.CreateFolderAsync(project.Id, rp.Path);
                    }
                    Logger.StatusBegin($"Creating permissions {rp.Path}...");
                    await ProvisionReleasePathPermissionsAsync(factory, project, rp, existingItem);
                    Logger.StatusEndSuccess("Succeed");
                }
            }
        }

        private async Task ProvisionReleasePathPermissionsAsync(
             Waddle.AdoConnectionFactory factory,
             Waddle.Dtos.Project project, PipelineFolder rp, VstsFolder existingItem)
        {
            if (existingItem != null)
            {
                var secService = factory.GetSecurityNamespaceService();
                var buildNamespace = await secService.GetNamespaceAsync(SecurityNamespaceConstants.ReleaseManagement, 
                    ReleaseManagementEx.AdministerReleasePermissions.ToString());
                var namespaceId = buildNamespace.NamespaceId;
                var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();
                await CreateAclsAsync(factory, typeof(Waddle.Constants.ReleaseManagementEx), rp.Permissions, aclDictioanry);

                if (aclDictioanry.Count > 0)
                {
                    var fpath = rp.Path.TrimStart("\\/".ToCharArray()).Replace("\\", "/");
                    var token = $"{project.Id}/{fpath}";
                    var aclService = factory.GetAclListService();
                    await aclService.SetAclsAsync(namespaceId, token, aclDictioanry, false);
                }
            }
        }

        private async Task EnsureBuildFoldersAsync(ProjectManifest manifest, 
            Waddle.AdoConnectionFactory factory, Waddle.Dtos.Project project)
        {
            if (manifest.BuildFolders != null && manifest.BuildFolders.Any())
            {
                var buildService = factory.GetBuildService();
                var buildPaths = await buildService.ListFoldersAsync(project.Id);
                foreach (var bp in manifest.BuildFolders)
                {
                    var existingItem = buildPaths.Value
                        .FirstOrDefault(p => p.Path.Replace("\\", "/").Equals(bp.Path, StringComparison.OrdinalIgnoreCase));

                    if (existingItem == null)
                    {
                        existingItem = await buildService.CreateFolderAsync(project.Id, bp.Path);
                    }
                    Logger.StatusBegin($"Creating permissions {bp.Path}...");
                    await ProvisionBuildPathPermissionsAsync(factory, project, bp, existingItem);
                    Logger.StatusEndSuccess("Succeed");
                }
            }
        }

        private async Task ProvisionBuildPathPermissionsAsync(
            Waddle.AdoConnectionFactory factory, 
            Waddle.Dtos.Project project, PipelineFolder bp, VstsFolder existingItem)
        {
            if (existingItem != null)
            {
                var secService = factory.GetSecurityNamespaceService();
                var buildNamespace = await secService.GetNamespaceAsync(SecurityNamespaceConstants.Build);
                var namespaceId = buildNamespace.NamespaceId;
                var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();
                await CreateAclsAsync(factory, typeof(Waddle.Constants.Build), bp.Permissions, aclDictioanry);

                if (aclDictioanry.Count > 0)
                {
                    var fpath = bp.Path.TrimStart("\\/".ToCharArray()).Replace("\\", "/");
                    var token = $"{project.Id}/{fpath}";
                    var aclService = factory.GetAclListService();
                    await aclService.SetAclsAsync(namespaceId, token, aclDictioanry, false);
                }
            }
        }

        private async Task EnsureEnvironmentExistsAsync(
            ProjectManifest manifest, 
            Waddle.AdoConnectionFactory factory, 
            Waddle.Dtos.Project project)
        {
            if (project != null && manifest.Environments != null && manifest.Environments.Any())
            {
                var peService = factory.GetPipelineEnvironmentService();
                foreach (var pe in manifest.Environments)
                {
                    if (pe != null && !string.IsNullOrWhiteSpace(pe.Name))
                    {
                        await ProvisionEnvironmentAsync(factory, project, peService, pe);              
                    }
                }
            }
        }

        private async Task ProvisionEnvironmentAsync(Waddle.AdoConnectionFactory factory, 
            Waddle.Dtos.Project project, 
            Waddle.PipelineEnvironmentService peService, EnvironmentManifest pe)
        {
            var peColl = await peService.ListEnvironmentsAsync(project.Id);
            if (peColl != null)
            {
                var envObject = peColl.Value
                     .FirstOrDefault(penv => penv.Name.Equals(pe.Name, StringComparison.OrdinalIgnoreCase));
                if (envObject == null)
                {
                    envObject = await peService.CreateEnvironmentAsync(project.Id, pe.Name, pe.Description);
                }

                await ProvisionEnvironmentPermissionsAsync(factory, project, peService, pe, envObject);
            }
        }

        private async Task ProvisionEnvironmentPermissionsAsync(
            Waddle.AdoConnectionFactory factory, 
            Waddle.Dtos.Project project, 
            Waddle.PipelineEnvironmentService peService, 
            EnvironmentManifest pe, PipelineEnvironment envObject)
        {
            if (envObject != null && pe.Permissions != null && pe.Permissions.Any())
            {
                foreach (var permissionObject in pe.Permissions)
                {
                    Logger.StatusBegin($"Configuring Environment ({pe.Name}) permissions: AAD object ({permissionObject.Group}) ...");
                    var group = await GetGroupByNameAsync(factory, 
                        permissionObject.Origin, permissionObject.Group, permissionObject.Id);
                    if (group != null)
                    {
                        var legacyIdentity = await factory.GetGroupService()
                            .GetLegacyIdentitiesBySidAsync(group.Sid);
                        if (legacyIdentity != null && legacyIdentity.Value.Any())
                        {
                            var localId = legacyIdentity.Value.First().Id;
                            foreach (var role in permissionObject.Roles)
                            {
                                await peService.SetPermissionAsync(project.Id, envObject.Id, localId, role);
                            }
                            Logger.StatusEndSuccess("Succeeded");
                        }
                    } 
                    else 
                    {
                        Logger.StatusEndFailed("Failed (Not found in AAD)");
                    }
                }
            }
        }

        private async Task EnsureRepositoriesExistsAsync(
            ProjectManifest manifest, Waddle.AdoConnectionFactory factory, 
            Waddle.RepositoryService repoService, 
            Waddle.Dtos.Project project,
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
                            Logger.StatusBegin($"Creating Repository {repo.Name}...");
                            await ExecutionSupports.Retry(async () =>
                                {
                                    repository = await repoService.CreateAsync(project.Id, repo.Name);
                                },
                                exception => { Logger.SilentError(exception.Message); });                                                        
                            Logger.StatusEndSuccess("Succeed");
                        }                        
                        Logger.StatusBegin($"Setting up permissions for repository {repo.Name}...");
                        await EnsureRepositoryPermissionsAsync(factory, project, repo, repository);
                        Logger.StatusEndSuccess("Succeed");
                    }
                }
                await DeleteDefaultRepoAsync(repoService, project, projectWasAbsent);
            }
        }

        private async Task EnsureRepositoryPermissionsAsync(
            Waddle.AdoConnectionFactory factory, Waddle.Dtos.Project project, 
            RepositoryManifest repo, Microsoft.TeamFoundation.SourceControl.WebApi.GitRepository repository)
        {
            if (repository != null && repo.Permissions != null && repo.Permissions.Any())
            {
                var secService = factory.GetSecurityNamespaceService();
                var gitNamespace = await secService.GetNamespaceAsync(SecurityNamespaceConstants.Git_Repositories);
                var gitSecurityNamespaceId = gitNamespace.NamespaceId;
                var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();
                await CreateAclsAsync(factory, typeof(GitRepositories), repo.Permissions, aclDictioanry);

                if (aclDictioanry.Count > 0)
                {
                    var repositorySecurityToken = $"repoV2/{project.Id}/{repository.Id}";
                    var aclService = factory.GetAclListService();
                    await aclService.SetAclsAsync(gitSecurityNamespaceId, repositorySecurityToken, aclDictioanry, false);
                }
            }
        }

        private async Task CreateAclsAsync(
            Waddle.AdoConnectionFactory factory,
            Type enumType,
            List<PermissionManifest> permissions, Dictionary<string, VstsAcesDictionaryEntry> aclDictioanry)
        {
            foreach (var permission in permissions)
            {
                if (permission.Allowed != null && permission.Allowed.Any())
                {
                    var group = await GetGroupByNameAsync(factory, permission.Origin, permission.Group);

                    if (group != null)
                    {
                        var bitMask = 0;
                        foreach (var permissionItem in permission.Allowed)
                        {
                            bitMask |= EnumSupport.GetBitMaskValue(enumType, permissionItem);
                        }
                        aclDictioanry.Add(group.Sid, new VstsAcesDictionaryEntry
                        {
                            Descriptor = group.Sid,
                            Allow = bitMask,
                            Deny = 0
                        });
                    }
                }
            }
        }

        private async Task<VstsGroup> GetGroupByNameAsync(
            Waddle.AdoConnectionFactory factory, string origin, string groupName, Guid? id = null)
        {
            var gService = factory.GetGroupService();
            var groups = await gService.ListGroupsAsync();
            var group = groups.Value
                .FirstOrDefault(g =>
                g.Origin.ToString().Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                g.PrincipalName.Contains(groupName, StringComparison.OrdinalIgnoreCase));

            if (group == null && id.HasValue)
            {
                group = await gService.CreateAadGroupByObjectId(id.Value);
            }
            return group;
        }

        private async Task<Tuple<Waddle.Dtos.Project, bool>> EnsureProjectExistsAsync(
            ProjectManifest manifest, Waddle.ProjectService projectService, 
            ProcessTemplate tempalte)
        {
            var projectCreatedJIT = false;
            var projects = await projectService.GetProjectsAsync();
            var project = projects.Value.FirstOrDefault(p => p.Name.Equals(manifest.Metadata.Name,
                StringComparison.OrdinalIgnoreCase));

            if (project == null)
            {
                await projectService.CreateProjectAsync(
                    manifest.Metadata.Name, tempalte,
                    manifest.Template.SourceControlType, manifest.Metadata.Description);
                projectCreatedJIT = true;
                Logger.StatusBegin("Waiting on project creation...");
                while (project == null)
                {
                    projects = await projectService.GetProjectsAsync();
                    project = projects.Value.FirstOrDefault(p => p.Name.Equals(manifest.Metadata.Name,
                                                StringComparison.OrdinalIgnoreCase));
                }
                Logger.StatusEndSuccess("Succeed");
            }
            else
            {
                Logger.Message($"{project.Name} already exists...");
            }
            return new Tuple<Waddle.Dtos.Project, bool>(project, projectCreatedJIT);
        }
    }
}
