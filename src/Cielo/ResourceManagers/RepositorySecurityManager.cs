﻿
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
        private readonly RepositoryService repositoryService;
        private readonly GraphService graphService;

        public RepositorySecurityManager(IServiceProvider serviceProvider, string rawManifest)
            : base(serviceProvider, rawManifest)
        {
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
                        var repoPropertyBag = new List<(string, object, bool)>();
                        state.AddProperty(repoName, repoPropertyBag);

                        foreach (var groupSpec in permissionSpec.Groups)
                        {
                            var group = await graphService.GetGroupAsync(
                                groupSpec.Name, project.Name,
                                groupSpec.Scope, groupSpec.Origin, groupSpec.AadObjectId);
                            if (group != null)
                            {
                                var childState = await DiscoverPermissionsAsync(project, group.Descriptor, repository, permissionSpec, readonlyMode);
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
                                var childState = await DiscoverPermissionsAsync(project, user.Descriptor, repository, permissionSpec, readonlyMode);
                                repoPropertyBag.Add((user.PrincipalName, childState.GetProperties(), false));
                            }
                            else
                            {
                                state.AddError($"{userSpec.Principal} not found!");
                            }
                        }
                    }
                    else
                    {
                        state.AddError($"{repoName} not found!");
                    }
                }
            }
            return state;
        }

        private async Task<ResourceState> DiscoverPermissionsAsync(
            Azdo.Dtos.Project project, 
            string descriptor,
            GitRepository repository,            
            RepositorySecurityManifest.RepositoryPermissionManifest permissionSpec,
            bool readonlyMode)
        {
            var state = new ResourceState() {  } ;
            var currentPerms = await repositoryService.GetPermissionsAsync(project.Id, descriptor, repository.Id);
            if (currentPerms != null)
            {
                if (readonlyMode)
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
                else
                {
                    // mutation mode
                    var acls = new Dictionary<string, VstsAcesDictionaryEntry>();
                    var bitMask = 0;
                    foreach (var expectedPermission in permissionSpec.Allowed)
                    {
                        bitMask |= EnumSupport.GetBitMaskValue(typeof(GitRepositories), expectedPermission.ToString());
                    }
                    var sid = IdentityBase64Supports.GetSid(descriptor);
                    if (!string.IsNullOrWhiteSpace(sid))
                    {
                        acls.Add(sid, new VstsAcesDictionaryEntry
                        {
                            Descriptor = sid,
                            Allow = bitMask,
                            Deny = 0
                        });
              
                        // update permissions


                    }
                    else
                    {
                        state.AddError($"Failed to calculate SID");
                    }
                }
            }
            return state;
        }

        public GitRepositories GetPermissionEnum()
        {
            var member = typeof(GitRepositories)
                .GetMembers()
                .FirstOrDefault((member) =>
                {
                    var dva = member.GetCustomAttribute<DefaultValueAttribute>();
                    if (dva != null)
                    {
                        if (int.TryParse($"{dva.Value}", out var value))
                        {
                            //return value == this.Bit;
                        }
                    }
                    return false;
                });

            return GitRepositories.EditPolicies;
        }

        protected override Type GetResourceType()
        {
            return typeof(RepositorySecurityManifest);
        }

        public RepositorySecurityManifest RepositorySecurityManifest { get { return (RepositorySecurityManifest)this.Manifest; } }
    }
}
