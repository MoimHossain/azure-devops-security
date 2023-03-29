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
    public class AreaPathSecurityManager : ResourceManagerBase
    {
        private readonly AclService aclService;        
        private readonly GraphService graphService;
        private readonly ClassificationService classificationService;

        public AreaPathSecurityManager(IServiceProvider serviceProvider, string rawManifest)
            : base(serviceProvider, rawManifest)
        {
            this.aclService = serviceProvider.GetRequiredService<AclService>();            
            this.graphService = serviceProvider.GetRequiredService<GraphService>();
            this.classificationService = serviceProvider.GetRequiredService<ClassificationService>();
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
            return typeof(AreaPathSecurityManifest);
        }

        public AreaPathSecurityManifest AreaPathSecurityManifest { get { return (AreaPathSecurityManifest)this.Manifest; } }


        private async Task<ResourceState> DiscoverAndApplyPermissionsAsync(bool readonlyMode = true)
        {
            var project = Context.CurrentProject;
            var metadataName = AreaPathSecurityManifest.Metadata.Name;
            var permissions = AreaPathSecurityManifest.Permissions;

            if (project == null || permissions == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is missing for resource {AreaPathSecurityManifest.Kind}:{metadataName}");
                return errorState;
            }

            var state = new ResourceState() { Exists = true };

            foreach (var permissionSpec in permissions)
            {
                if(permissionSpec.Paths != null)
                {
                    foreach (var areaPathString in permissionSpec.Paths)
                    {
                        var areaPathObject = await this.classificationService.GetAreaPathAsync(project.Id, areaPathString);
                        if (areaPathObject != null)
                        {
                            await DiscoverAndApplyPermissionsOnFolderAsync(readonlyMode, project, state, permissionSpec, areaPathObject);
                        }
                        else
                        {
                            if (readonlyMode)
                            {
                                state.AddProperty($"PATH({areaPathString})", "Missing, will be created.");
                            }
                            else
                            {
                                areaPathObject = await this.classificationService.CreateOrUpdateAreaPathAsync(project.Id, areaPathString);
                                if (areaPathObject != null)
                                {
                                    state.AddProperty("Created", $"PATH({areaPathString})");
                                    await DiscoverAndApplyPermissionsOnFolderAsync(readonlyMode, project, state, permissionSpec, areaPathObject);
                                }
                                else
                                {
                                    state.AddError($"{areaPathString} creation failed.");
                                }
                            }
                        }
                    }
                }
            }
            return state;
        }

        private async Task DiscoverAndApplyPermissionsOnFolderAsync(
            bool readonlyMode, Azdo.Dtos.Project project, ResourceState state,
            AreaPathSecurityManifest.AreaPathSecurityPermissionManifest permissionSpec,
            VstsClassification pathSpec)
        {
            var clsNodePropertyBag = new List<(string, object, bool)>();
            state.AddProperty($"PATH({pathSpec.TrimmedPath})", clsNodePropertyBag);
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
                            project, group.Descriptor, group.Sid, pathSpec, permissionSpec, readonlyMode, acls);
                        clsNodePropertyBag.Add((group.PrincipalName, childState.GetProperties(), false));
                    }
                    else
                    {
                        state.AddError($"{groupSpec.Name} not found!");
                    }
                }
            }            

            if(permissionSpec.Users != null)
            {
                foreach (var userSpec in permissionSpec.Users)
                {
                    var user = await graphService.GetUserByPrincipalNameAsync(userSpec.Principal);
                    if (user != null)
                    {
                        var childState = await DiscoverPermissionsAsync(
                            project, user.Descriptor, user.Sid, pathSpec, permissionSpec, readonlyMode, acls);
                        clsNodePropertyBag.Add((user.PrincipalName, childState.GetProperties(), false));
                    }
                    else
                    {
                        state.AddError($"{userSpec.Principal} not found!");
                    }
                }
            }

            if (!readonlyMode && acls.Any())
            {
                var result = await classificationService.SetAreaPathPermissionsAsync(project.Id, pathSpec, acls.Values.ToList());

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
            VstsClassification clsNode,
            AreaPathSecurityManifest.AreaPathSecurityPermissionManifest permissionSpec,
            bool readonlyMode,
            Dictionary<string, VstsAcesDictionaryEntry> acls)
        {
            var state = new ResourceState() { };
            var currentPerms = await classificationService.GetAreaPathPermissionsAsync(project.Id, descriptor, clsNode.Identifier);
            if (currentPerms != null && permissionSpec.Allowed != null)
            {
                if (readonlyMode)
                {
                    foreach (var expectedPermission in permissionSpec.Allowed)
                    {
                        var bit = EnumSupport.GetBitMaskValue(typeof(PermissionEnums.CSS), expectedPermission.ToString());
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
                            bitMask |= EnumSupport.GetBitMaskValue(typeof(PermissionEnums.CSS), expectedPermission.ToString());
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
