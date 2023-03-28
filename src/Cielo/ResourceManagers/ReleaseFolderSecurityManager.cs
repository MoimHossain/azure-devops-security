using Cielo.Azdo;
using Cielo.Azdo.Dtos;
using Cielo.Manifests;
using Cielo.ResourceManagers.Abstract;
using Cielo.ResourceManagers.ResourceStates;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.ResourceManagers
{
    public class ReleaseFolderSecurityManager : ResourceManagerBase
    {
        private readonly AclService aclService;
        private readonly ReleaseService releaseService;
        private readonly GraphService graphService;

        public ReleaseFolderSecurityManager(IServiceProvider serviceProvider, string rawManifest)
            : base(serviceProvider, rawManifest)
        {
            this.aclService = serviceProvider.GetService<AclService>();
            this.releaseService = serviceProvider.GetRequiredService<ReleaseService>();
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

        protected override Type GetResourceType()
        {
            return typeof(ReleaseFolderSecurityManifest);
        }

        public ReleaseFolderSecurityManifest ReleaseFolderSecurityManifest { get { return (ReleaseFolderSecurityManifest)this.Manifest; } }


        private async Task<ResourceState> DiscoverAndApplyPermissionsAsync(bool readonlyMode = true)
        {
            var project = Context.CurrentProject;
            var metadataName = ReleaseFolderSecurityManifest.Metadata.Name;
            var permissions = ReleaseFolderSecurityManifest.Permissions;

            if (project == null || permissions == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is missing for resource {ReleaseFolderSecurityManifest.Kind}:{metadataName}");
                return errorState;
            }

            var state = new ResourceState() { Exists = true };

            foreach (var permissionSpec in permissions)
            {
                foreach (var pipelineFolderPath in permissionSpec.Paths)
                {
                    var folderPath = await this.releaseService.GetPipelineFolderAsync(project.Id, pipelineFolderPath);
                    if (folderPath != null)
                    {
                        await DiscoverAndApplyPermissionsOnFolderAsync(readonlyMode, project, state, permissionSpec, folderPath);
                    }
                    else
                    {
                        if (readonlyMode)
                        {
                            state.AddProperty($"PATH({pipelineFolderPath})", "Missing, will be created.");
                        }
                        else
                        {
                            folderPath = await this.releaseService.CreateFolderAsync(project.Id, pipelineFolderPath);
                            if (folderPath != null)
                            {
                                state.AddProperty($"PATH({pipelineFolderPath})", "Created");
                                await DiscoverAndApplyPermissionsOnFolderAsync(readonlyMode, project, state, permissionSpec, folderPath);
                            }
                            else
                            {
                                state.AddError($"{pipelineFolderPath} creation failed.");
                            }
                        }

                    }
                }
            }
            return state;
        }

        private async Task DiscoverAndApplyPermissionsOnFolderAsync(
            bool readonlyMode, Azdo.Dtos.Project project, ResourceState state,
            ReleaseFolderSecurityManifest.ReleaseFolderPermissionManifest permissionSpec,
            VstsFolder folder)
        {
            var repoPropertyBag = new List<(string, object, bool)>();
            state.AddProperty($"PATH({folder.Path})", repoPropertyBag);
            var acls = new Dictionary<string, VstsAcesDictionaryEntry>();

            foreach (var groupSpec in permissionSpec.Groups)
            {
                var group = await graphService.GetGroupAsync(
                    groupSpec.Name, project.Name,
                    groupSpec.Scope, groupSpec.Origin, groupSpec.AadObjectId);
                if (group != null)
                {
                    var childState = await DiscoverPermissionsAsync(
                        project, group.Descriptor, group.Sid, folder, permissionSpec, readonlyMode, acls);
                    repoPropertyBag.Add((group.PrincipalName, childState.GetProperties(), false));
                }
                else
                {
                    state.AddError($"{groupSpec.Name} not found!");
                }
            }

            foreach (var userSpec in permissionSpec.Users)
            {
                var user = await graphService.GetUserByPrincipalNameAsync(userSpec.Principal);
                if (user != null)
                {
                    var childState = await DiscoverPermissionsAsync(
                        project, user.Descriptor, user.Sid, folder, permissionSpec, readonlyMode, acls);
                    repoPropertyBag.Add((user.PrincipalName, childState.GetProperties(), false));
                }
                else
                {
                    state.AddError($"{userSpec.Principal} not found!");
                }
            }


            if (!readonlyMode && acls.Any())
            {
                var nsID = await this.releaseService.GetNamespaceId();
                var securityToken = releaseService.GetSecurityTokenForPath(project.Id, folder.Path);
                var result = await aclService.SetAclsAsync(nsID, securityToken, acls, false);
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
            VstsFolder folder,
            ReleaseFolderSecurityManifest.ReleaseFolderPermissionManifest permissionSpec,
            bool readonlyMode,
            Dictionary<string, VstsAcesDictionaryEntry> acls)
        {
            var state = new ResourceState() { };
            var currentPerms = await releaseService.GetFolderPermissionsAsync(project.Id, descriptor, folder.Path);
            if (currentPerms != null)
            {
                if (readonlyMode)
                {
                    foreach (var expectedPermission in permissionSpec.Allowed)
                    {
                        var bit = EnumSupport.GetBitMaskValue(typeof(PermissionEnums.ReleaseManagementEx), expectedPermission.ToString());
                        var crrentPerm = currentPerms.FirstOrDefault(cp => cp.Bit == bit);
                        if (crrentPerm != null && !string.IsNullOrWhiteSpace(crrentPerm.PermissionDisplayString))
                        {
                            if (readonlyMode)
                            {
                                var missingExpectation = !crrentPerm.PermissionDisplayString.Contains("Allow");
                                state.AddProperty(expectedPermission.ToString(), crrentPerm.PermissionDisplayString, missingExpectation);
                            }
                        }
                    }
                }
                else
                {

                    if (!string.IsNullOrWhiteSpace(sid))
                    {
                        // mutation mode                    
                        var bitMask = 0;
                        foreach (var expectedPermission in permissionSpec.Allowed)
                        {
                            bitMask |= EnumSupport.GetBitMaskValue(typeof(PermissionEnums.ReleaseManagementEx), expectedPermission.ToString());
                        }
                        acls.Add(sid, new VstsAcesDictionaryEntry
                        {
                            Descriptor = sid,
                            Allow = bitMask,
                            Deny = 0
                        });
                    }
                    else
                    {
                        state.AddError($"Failed to calculate SID");
                    }
                }
            }
            return state;
        }
    }
}
