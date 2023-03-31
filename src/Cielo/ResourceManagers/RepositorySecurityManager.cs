
using Cielo.Azdo;
using Cielo.Azdo.Dtos;
using Cielo.Manifests;
using Cielo.ResourceManagers.Abstract;
using Cielo.ResourceManagers.ResourceStates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Reflection;
using static Cielo.Azdo.PermissionEnums;

namespace Cielo.ResourceManagers
{
    public class RepositorySecurityManager : ResourceManagerBase
    {
        private readonly AclService aclService;
        private readonly RepositoryService repositoryService;
        private readonly GraphService graphService;

        public RepositorySecurityManager(IServiceProvider serviceProvider, string rawManifest)
            : base(serviceProvider, rawManifest)
        {
            this.aclService = serviceProvider.GetService<AclService>();
            this.repositoryService = serviceProvider.GetRequiredService<RepositoryService>();
            this.graphService = serviceProvider.GetRequiredService<GraphService>();
        }

        protected async override Task<ResourceState> CreateAsync()
        {
            return await DiscoverAndApplyPermissionsAsync(false);
        }


        protected async override Task<ResourceState?> UpdateAsync()
        {
            return await DiscoverAndApplyPermissionsAsync(false);
        }

        protected async override Task<ResourceState> GetAsync()
        {
            return await DiscoverAndApplyPermissionsAsync();
        }

        private async Task<ResourceState> DiscoverAndApplyPermissionsAsync(bool readonlyMode = true)
        {
            var project = Context.CurrentProject;
            var metadataName = RepositorySecurityManifest.Metadata.Name;
            var permissions = RepositorySecurityManifest.Permissions;

            if (project == null || permissions == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is missing for resource {RepositorySecurityManifest.Kind}:{metadataName}");
                return errorState;
            }

            var state = new ResourceState() { Exists = true };

            foreach (var permissionSpec in permissions)
            {
                foreach (var repoName in permissionSpec.Names)
                {
                    var repository = await this.repositoryService.GetRepositoryAsync(project.Id, repoName);
                    if (repository != null)
                    {
                        await DiscoverAndApplyPermissionsOnRepoAsync(readonlyMode, project, state, permissionSpec, repoName, repository);
                    }
                    else
                    {
                        state.AddError($"{repoName} not found!");
                    }
                }
            }
            return state;
        }

        private async Task DiscoverAndApplyPermissionsOnRepoAsync(
            bool readonlyMode, Azdo.Dtos.Project project, ResourceState state, 
            RepositorySecurityManifest.RepositoryPermissionManifest permissionSpec, 
            string repoName, GitRepository? repository)
        {
            var repoPropertyBag = new List<(string, object, bool)>();
            state.AddProperty(repoName, repoPropertyBag);
            var acls = new Dictionary<string, VstsAcesDictionaryEntry>();

            if(permissionSpec.Groups != null)
            {
                foreach (var groupSpec in permissionSpec.Groups)
                {
                    var group = await graphService.GetGroupAsync(
                        groupSpec.Name, project.Name,
                        groupSpec.Scope, groupSpec.Origin, groupSpec.AadObjectId);
                    if (group != null)
                    {
                        var childState = await DiscoverPermissionsAsync(
                            project, group.Descriptor, group.Sid, repository, permissionSpec, readonlyMode, acls);
                        repoPropertyBag.Add((group.PrincipalName, childState.GetProperties(), false));
                    }
                    else
                    {
                        state.AddError($"{groupSpec.Name} not found!");
                    }
                }
            }


            if (permissionSpec.Users != null)
            {
                foreach (var userSpec in permissionSpec.Users)
                {
                    var user = await graphService.GetUserByPrincipalNameAsync(userSpec.Principal);
                    if (user != null)
                    {
                        var childState = await DiscoverPermissionsAsync(
                            project, user.Descriptor, user.Sid, repository, permissionSpec, readonlyMode, acls);
                        repoPropertyBag.Add((user.PrincipalName, childState.GetProperties(), false));
                    }
                    else
                    {
                        state.AddError($"{userSpec.Principal} not found!");
                    }
                }
            }


            if (!readonlyMode && acls.Any())
            {
                var gitSecurityNamespaceId = await this.repositoryService.GetNamespaceId();
                var repositorySecurityToken = $"repoV2/{project.Id}/{repository.Id}";
                var result = await aclService.SetAclsAsync(gitSecurityNamespaceId, repositorySecurityToken, acls, false);
                if (!result)
                {
                    state.AddError("Failed to update permissions.");
                }
                else
                {
                    state.AddProperty("Permissions", "Applied successfully.");
                }
            }
        }

        private async Task<ResourceState> DiscoverPermissionsAsync(
            Azdo.Dtos.Project project, 
            string descriptor,
            string sid,
            GitRepository repository,            
            RepositorySecurityManifest.RepositoryPermissionManifest permissionSpec,
            bool readonlyMode,
            Dictionary<string, VstsAcesDictionaryEntry> acls)
        {
            var state = new ResourceState() {  } ;
            var currentPerms = await repositoryService.GetPermissionsAsync(project.Id, descriptor, repository.Id);
            if (currentPerms != null)
            {
                if (readonlyMode)
                {
                    if(permissionSpec.Allowed != null )
                    {
                        foreach (var expectedPermission in permissionSpec.Allowed)
                        {
                            var bit = EnumSupport.GetBitMaskValue(typeof(GitRepositories), expectedPermission.ToString());
                            var crrentPerm = currentPerms.FirstOrDefault(cp => cp.Bit == bit);
                            if (crrentPerm != null && crrentPerm.EffectivePermissionValue.HasValue)
                            {
                                if (readonlyMode)
                                {
                                    var missingExpectation = ((crrentPerm.EffectivePermissionValue.Value & bit) <= 0);
                                    state.AddProperty(expectedPermission.ToString(), (missingExpectation ? "Missing" : "Allowed"), missingExpectation);
                                }
                            }
                        }
                    }

                }
                else
                {
                                        
                    if (!string.IsNullOrWhiteSpace(sid))
                    {
                        if(permissionSpec.Allowed != null )
                        {
                            // mutation mode                    
                            var bitMask = 0;
                            foreach (var expectedPermission in permissionSpec.Allowed)
                            {
                                bitMask |= EnumSupport.GetBitMaskValue(typeof(GitRepositories), expectedPermission.ToString());
                            }
                            acls.Add(sid, new VstsAcesDictionaryEntry
                            {
                                Descriptor = sid,
                                Allow = bitMask,
                                Deny = 0
                            });
                        }

                    }
                    else
                    {
                        state.AddError($"Failed to calculate SID");
                    }
                }
            }
            return state;
        }

        protected override Type GetResourceType()
        {
            return typeof(RepositorySecurityManifest);
        }

        public RepositorySecurityManifest RepositorySecurityManifest { get { return (RepositorySecurityManifest)this.Manifest; } }
    }
}
