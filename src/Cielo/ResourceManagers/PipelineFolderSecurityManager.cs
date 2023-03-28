using Cielo.Azdo;
using Cielo.Azdo.Dtos;
using Cielo.Manifests;
using Cielo.ResourceManagers.Abstract;
using Cielo.ResourceManagers.ResourceStates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.ResourceManagers
{
    public class PipelineFolderSecurityManager : ResourceManagerBase
    {
        private readonly AclService aclService;
        private readonly PipelineService pipelineService;
        private readonly GraphService graphService;

        public PipelineFolderSecurityManager(IServiceProvider serviceProvider, string rawManifest) 
            : base(serviceProvider, rawManifest)
        {
            this.aclService = serviceProvider.GetService<AclService>();
            this.pipelineService = serviceProvider.GetRequiredService<PipelineService>();
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
            return typeof(PipelineFolderSecurityManifest);
        }

        public PipelineFolderSecurityManifest PipelineFolderSecurityManifest { get { return (PipelineFolderSecurityManifest)this.Manifest; } }


        private async Task<ResourceState> DiscoverAndApplyPermissionsAsync(bool readonlyMode = true)
        {
            var project = Context.CurrentProject;
            var metadataName = PipelineFolderSecurityManifest.Metadata.Name;
            var permissions = PipelineFolderSecurityManifest.Permissions;

            if (project == null || permissions == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is missing for resource {PipelineFolderSecurityManifest.Kind}:{metadataName}");
                return errorState;
            }

            var state = new ResourceState() { Exists = true };

            foreach (var permissionSpec in permissions)
            {
                foreach (var pipelineFolderPath in permissionSpec.Paths)
                {
                    var folderPath = await this.pipelineService.GetPipelineFolderAsync(project.Id, pipelineFolderPath);
                    if (folderPath != null)
                    {
                        await DiscoverAndApplyPermissionsOnFolderAsync(readonlyMode, project, state, permissionSpec, folderPath);
                    }
                    else
                    {
                        state.AddError($"{pipelineFolderPath} not found!");
                    }
                }
            }
            return state;
        }

        private async Task DiscoverAndApplyPermissionsOnFolderAsync(
            bool readonlyMode, Azdo.Dtos.Project project, ResourceState state,
            PipelineFolderSecurityManifest.PipelineFolderPermissionManifest permissionSpec,
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
                var nsID = await this.pipelineService.GetNamespaceId();
                var securityToken = pipelineService.GetSecurityTokenForPath(project.Id, folder.Path);
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
            PipelineFolderSecurityManifest.PipelineFolderPermissionManifest permissionSpec,
            bool readonlyMode,
            Dictionary<string, VstsAcesDictionaryEntry> acls)
        {
            var state = new ResourceState() { };
            var currentPerms = await pipelineService.GetFolderPermissionsAsync(project.Id, descriptor, folder.Path);
            if (currentPerms != null)
            {
                if (readonlyMode)
                {
                    foreach (var expectedPermission in permissionSpec.Allowed)
                    {
                        var bit = EnumSupport.GetBitMaskValue(typeof(PermissionEnums.Build), expectedPermission.ToString());
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
                else
                {

                    if (!string.IsNullOrWhiteSpace(sid))
                    {
                        // mutation mode                    
                        var bitMask = 0;
                        foreach (var expectedPermission in permissionSpec.Allowed)
                        {
                            bitMask |= EnumSupport.GetBitMaskValue(typeof(PermissionEnums.Build), expectedPermission.ToString());
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
