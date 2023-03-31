

using Cielo.Azdo;
using Cielo.Azdo.Dtos;
using Cielo.Manifests;
using Cielo.ResourceManagers.Abstract;
using Cielo.ResourceManagers.ResourceStates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualBasic;
using System;
using static Cielo.Azdo.PermissionEnums;

namespace Cielo.ResourceManagers
{
    public class EnvSecurityManager : ResourceManagerBase
    {
        private readonly AclService aclService;
        private readonly GraphService graphService;
        private readonly EnvironmentService environmentService;

        public EnvSecurityManager(IServiceProvider serviceProvider, string rawManifest)
            : base(serviceProvider, rawManifest)
        {
            this.aclService = serviceProvider.GetRequiredService<AclService>();
            this.graphService = serviceProvider.GetRequiredService<GraphService>();
            this.environmentService = serviceProvider.GetRequiredService<EnvironmentService>();
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
            var metadataName = EnvSecurityManifest.Metadata.Name;
            var permissions = EnvSecurityManifest.Permissions;

            if (project == null || permissions == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is missing for resource {EnvSecurityManifest.Kind}:{metadataName}");
                return errorState;
            }

            var state = new ResourceState() { Exists = true };

            foreach (var permissionSpec in permissions)
            {
                await DiscoverAndApplyRootPermissionsAsync(readonlyMode, project, state, permissionSpec);

                await CreateAndUpdatePermissionsForEnvsAsync(readonlyMode, project, state, permissionSpec);
            }
            return state;
        }

        private async Task CreateAndUpdatePermissionsForEnvsAsync(bool readonlyMode, Azdo.Dtos.Project project, ResourceState state, EnvSecurityManifest.EnvPermissionManifest permissionSpec)
        {
            if (permissionSpec.Names != null)
            {
                foreach (var envName in permissionSpec.Names)
                {
                    var envObject = await this.environmentService.GetEnvironmentAsync(project.Id, envName);
                    if (envObject != null)
                    {
                        await DiscoverAndApplyPermissionsOnEnvironmentAsync(readonlyMode, project, state, permissionSpec, envName, envObject);
                    }
                    else
                    {
                        if (readonlyMode)
                        {
                            state.AddProperty($"{envName}", "Will be created.", true);
                        }
                        else
                        {
                            envObject = await environmentService.CreateEnvironmentAsync(project.Id, envName);
                            if (envObject == null)
                            {
                                state.AddError($"{envName} failed to create.");
                            }
                            else
                            {
                                var envSpecficState = new ResourceState();
                                await DiscoverAndApplyPermissionsOnEnvironmentAsync(readonlyMode, project, envSpecficState, permissionSpec, envName, envObject);
                                                               
                                state.AddProperty($"ENV ({envObject.Name})", envSpecficState.GetProperties().ToList());
                                foreach (var error in envSpecficState.GetErrors())
                                {
                                    state.AddError(error);
                                }
                            }
                        }
                    }
                }
            }
        }

        private async Task DiscoverAndApplyRootPermissionsAsync(bool readonlyMode, Azdo.Dtos.Project project, ResourceState state, EnvSecurityManifest.EnvPermissionManifest permissionSpec)
        {
            if (permissionSpec.ApplyToRoot)
            {
                var rootState = new ResourceState();

                var permissionMutations = new List<(Guid, string, string)>();
                var currentRootPermissions = await environmentService.GetRootPermissionsAsync(project.Id);

                await DiscoverAndApplyGroupPermissionsAsync(readonlyMode, project, rootState, permissionSpec, permissionMutations, currentRootPermissions);

                await DiscoverAndApplyUserPermissionsAsync(readonlyMode, rootState, permissionSpec, permissionMutations, currentRootPermissions);

                await ApplyPermissionsCoreAsync(readonlyMode, project, rootState, null, permissionMutations, true);

                state.AddProperty($"ENV (ROOT)", rootState.GetProperties().ToList());
                foreach(var error in rootState.GetErrors())
                {
                    state.AddError(error);
                }
            }
        }

        private async Task DiscoverAndApplyPermissionsOnEnvironmentAsync(
            bool readonlyMode, Azdo.Dtos.Project project, ResourceState state,
            EnvSecurityManifest.EnvPermissionManifest permissionSpec,
            string envName, VstsEnvironment? environment)
        {
            if (environment == null)
            {
                state.AddError("Failed to read permissions");
                return;
            }
            var envPropertyBag = new List<(string, object, bool)>();
            state.AddProperty(envName, envPropertyBag);
            var permissionMutations = new List<(Guid, string, string)>();

            var currentPermissions = await environmentService.GetEnvPermissionsAsync(project.Id, environment.Id);
            if (currentPermissions != null && permissionSpec.Roles != null)
            {
                await DiscoverAndApplyGroupPermissionsAsync(readonlyMode, project, state, permissionSpec, permissionMutations, currentPermissions);

                await DiscoverAndApplyUserPermissionsAsync(readonlyMode, state, permissionSpec, permissionMutations, currentPermissions);
            }

            await ApplyPermissionsCoreAsync(readonlyMode, project, state, environment, permissionMutations, false);
        }

        private async Task DiscoverAndApplyGroupPermissionsAsync(
            bool readonlyMode, 
            Azdo.Dtos.Project project, ResourceState state, 
            EnvSecurityManifest.EnvPermissionManifest permissionSpec, 
            List<(Guid, string, string)> permissionMutations, 
            List<VstsEnvPermission>? currentPermissions)
        {
            if (permissionSpec.Groups != null && currentPermissions != null)
            {
                foreach (var groupSpec in permissionSpec.Groups)
                {
                    var groupId = await graphService.GetGroupIdByGroupAsync(
                        groupSpec.Name, project.Name,
                        groupSpec.Scope, groupSpec.Origin, groupSpec.AadObjectId);
                    if (groupId != null)
                    {
                        foreach (var role in permissionSpec.Roles)
                        {
                            var gpPermissionExists = currentPermissions
                                    .FirstOrDefault(cp =>
                                        cp.Identity.Id.Equals($"{groupId}") &&
                                        cp.Role.Name.Equals(role.ToString(), StringComparison.OrdinalIgnoreCase));

                            if (gpPermissionExists == null)
                            {
                                if (!readonlyMode)
                                {
                                    permissionMutations.Add((groupId.Value, groupSpec.Name, role.ToString()));
                                }
                                else
                                {
                                    state.AddProperty($"{groupSpec.Name}", $"Permission will be added.");
                                }
                            }
                            else
                            {
                                if (readonlyMode)
                                {
                                    state.AddProperty($"{groupSpec.Name}", "Permission Exists");
                                }
                                else {
                                    state.AddProperty($"{groupSpec.Name}", "No changes");
                                }

                            }
                        }
                    }
                    else
                    {
                        state.AddError($"{groupSpec.Name} not found!");
                    }
                }
            }
        }

        private async Task DiscoverAndApplyUserPermissionsAsync(
            bool readonlyMode, ResourceState state, 
            EnvSecurityManifest.EnvPermissionManifest permissionSpec, 
            List<(Guid, string, string)> permissionMutations, 
            List<VstsEnvPermission>? currentPermissions)
        {
            if (permissionSpec.Users != null && currentPermissions != null)
            {
                foreach (var userSpec in permissionSpec.Users)
                {
                    var userId = await graphService.GetUserIdByPrincipalNameAsync(userSpec.Principal);
                    if (userId != null)
                    {
                        foreach (var role in permissionSpec.Roles)
                        {
                            var gpPermissionExists = currentPermissions
                                    .FirstOrDefault(cp =>
                                        cp.Identity.Id.Equals($"{userId}") &&
                                        cp.Role.Name.Equals(role.ToString(), StringComparison.OrdinalIgnoreCase));

                            if (gpPermissionExists == null)
                            {
                                if (!readonlyMode)
                                {
                                    permissionMutations.Add((userId.Value, userSpec.Name, role.ToString()));
                                }
                                else
                                {
                                    state.AddProperty($"{userSpec.Name}", $"Permission will be added.");
                                }
                            }
                            else
                            {
                                if (readonlyMode)
                                {
                                    state.AddProperty($"{userSpec.Name}", "Permission Exists");
                                }
                                else {
                                    state.AddProperty($"{userSpec.Name}", "No changes");
                                }
                            }
                        }
                    }
                    else
                    {
                        state.AddError($"{userSpec.Name} not found!");
                    }
                }
            }
        }

        private async Task ApplyPermissionsCoreAsync(
            bool readonlyMode, 
            Azdo.Dtos.Project project, 
            ResourceState state, 
            VstsEnvironment? environment, 
            List<(Guid, string, string)> permissionMutations, bool applyRootPermissions)
        {
            if (!readonlyMode && permissionMutations.Any())
            {
                var changes = permissionMutations.Select(pm =>
                    new VstsEnvRoleAssignment
                    {
                        userId = pm.Item1,
                        roleName = pm.Item3
                    }).ToList();
                var success = false;
                if (applyRootPermissions)
                {
                    success = await environmentService.AddRootPermissionsAsync(project.Id, changes);
                }
                else
                {
                    success = await environmentService.AddPermissionsAsync(project.Id, environment.Id, changes);
                }
        
                if (!success)
                {
                    state.AddError("Failed to update permissions!");
                }
                else
                {
                    var name = environment != null ? environment.Name : "Root";
                    state.AddProperty($"{name}", "Permission updated.");
                }
            }
        }

        protected override Type GetResourceType()
        {
            return typeof(EnvSecurityManifest);
        }

        public EnvSecurityManifest EnvSecurityManifest { get { return (EnvSecurityManifest)this.Manifest; } }
    }
}
