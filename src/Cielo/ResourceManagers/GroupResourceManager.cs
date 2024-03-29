﻿using Cielo.Azdo;
using Cielo.Azdo.Dtos;
using Cielo.Manifests;
using Cielo.ResourceManagers.Abstract;
using Cielo.ResourceManagers.ResourceStates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.ResourceManagers
{
    public class GroupResourceManager : ResourceManagerBase
    {
        private readonly GraphService graphService;

        public GroupResourceManager(IServiceProvider serviceProvider, string rawManifest) : base(serviceProvider, rawManifest)
        {
            this.graphService = serviceProvider.GetRequiredService<GraphService>();
        }

        protected async override Task<ResourceState> CreateAsync()
        {
            var project = Context.CurrentProject;
            var groupName = GroupManifest.Metadata.Name;
            var description = GroupManifest.Metadata.Description;
            if (project == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is issing for resource {GroupManifest.Kind}:{groupName}");
                return errorState;
            }

            var group = default(VstsGroup);
            var state = new ResourceState();
            if (string.IsNullOrEmpty(groupName) || GroupManifest == null || GroupManifest.Properties == null)
            {
                state.AddError("Group name is empty");
                return state;
            }

            if(GroupManifest.Properties.Origin == IdentityOrigin.Aad)
            {
                if(GroupManifest.Properties.AadObjectId == null)
                {
                    state.AddError($"Group ({groupName}) with origin({IdentityOrigin.Aad.ToString()}) requires `AadObjectId` provided.");
                    return state;
                }

                group = await graphService.CreateAadGroupByObjectId(GroupManifest.Properties.AadObjectId.Value);
            }
            else
            {
                if (GroupManifest.Properties.Scope == GroupManifest.GroupScopeEnum.Project)
                {
                    group = await graphService.CreateProjectScopedVstsGroupAsync(project.Id, groupName, description);
                }
                else 
                {
                    group = await graphService.CreateOrgScopedVstsGroupAsync(groupName, description);
                }
            }

            if(group != null)
            {
                state.AddProperty("PrincipalName", group.PrincipalName);
                state.AddProperty("Origin", group.Origin);
                state.AddProperty("OriginId", group.OriginId);
                state.AddProperty("PrincipalName", group.PrincipalName);
                state.AddProperty("Descriptor", $"{group.Descriptor.Substring(0, 5)} ...(truncated)");

                await UpdateMembershipAsync(group, state, project, GroupManifest.Properties.Scope, GroupManifest.Properties.Origin);
            }
            return state;
        }

        protected async override Task<ResourceState> GetAsync()
        {
            var project = Context.CurrentProject;
            var groupName = GroupManifest.Metadata.Name;
            if (project == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is issing for resource {GroupManifest.Kind}:{groupName}");
                return errorState;
            }

            var state = new ResourceState();
            if (string.IsNullOrEmpty(groupName) || GroupManifest == null || GroupManifest.Properties == null)
            {
                state.AddError("Group name is empty");
                return state;
            }

            var group = await graphService.GetGroupAsync(
                groupName, project.Name, 
                GroupManifest.Properties.Scope, 
                GroupManifest.Properties.Origin,
                GroupManifest.Properties.AadObjectId);
            state.AddProperty("Name", groupName);
            state.AddProperty("Origin", GroupManifest.Properties.Origin.ToString());
            state.AddProperty("Scope", GroupManifest.Properties.Scope.ToString());

            if (group == null)
            {
                state.Exists = false;
            }
            else
            {
                state.Exists = true;
                await DiscoverMembershipAsync(group, state, project, GroupManifest.Properties.Scope, GroupManifest.Properties.Origin);
            }
            return state;
        }


        protected async override Task<ResourceState?> UpdateAsync()
        {
            var project = Context.CurrentProject;
            var groupName = GroupManifest.Metadata.Name;
            if (project == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is issing for resource {GroupManifest.Kind}:{groupName}");
                return errorState;
            }

            var state = new ResourceState();
            if (string.IsNullOrEmpty(groupName) || GroupManifest == null || GroupManifest.Properties == null)
            {
                state.AddError("Group name is empty");
                return state;
            }

            var group = await graphService.GetGroupAsync(
                groupName, project.Name, 
                GroupManifest.Properties.Scope, 
                GroupManifest.Properties.Origin, GroupManifest.Properties.AadObjectId);

            if(group != null)
            {
                await UpdateMembershipAsync(group, state, project, GroupManifest.Properties.Scope, GroupManifest.Properties.Origin);
            }

            return state;
        }

        #region Helper methods

        private async Task UpdateMembershipAsync(
            VstsGroup group,
            ResourceState state,
            Project project,
            GroupManifest.GroupScopeEnum scope,
            IdentityOrigin origin)
        {
            var membership = await graphService.GetGroupMembersAsync(group.Descriptor);

            if (membership != null && GroupManifest.Membership != null)
            {
                if (GroupManifest.Membership.Groups != null)
                {
                    foreach (var expectedGroup in GroupManifest.Membership.Groups)
                    {
                        var foundGroup = await graphService.GetGroupAsync(expectedGroup.Name, project.Name, expectedGroup.Scope, expectedGroup.Origin, expectedGroup.AadObjectId);

                        if (foundGroup != null)
                        {
                            var alreadyMember = membership.Members
                                .FirstOrDefault(m => m.MemberDescriptor.Equals(foundGroup.Descriptor, StringComparison.OrdinalIgnoreCase));

                            if (alreadyMember == null)
                            {
                                if (await graphService.AddMemberAsync(project.Id, group.Descriptor, foundGroup.Descriptor))
                                {
                                    state.AddProperty($"Membership", $"{foundGroup.PrincipalName} added as member", false);
                                }
                                else
                                {
                                    state.AddError($"Failed to add ({foundGroup.PrincipalName}) as member");
                                }
                            }
                        }
                    }
                }

                if (GroupManifest.Membership.Users != null)
                {
                    foreach (var expectedUser in GroupManifest.Membership.Users)
                    {
                        var foundUser = await graphService.GetUserByPrincipalNameAsync(expectedUser.Principal);

                        if (foundUser != null)
                        {
                            var alreadyMember = membership.Members
                                .FirstOrDefault(m => m.MemberDescriptor.Equals(foundUser.Descriptor, StringComparison.OrdinalIgnoreCase));

                            if (alreadyMember == null)
                            {
                                if (await graphService.AddMemberAsync(project.Id, group.Descriptor, foundUser.Descriptor))
                                {
                                    state.AddProperty($"Membership", $"{foundUser.PrincipalName} added as member", false);
                                }
                                else
                                {
                                    state.AddError($"Failed to add ({foundUser.PrincipalName}) as member");
                                }
                            }
                        }
                    }
                }
            }
        }

        private async Task DiscoverMembershipAsync(
            VstsGroup group,
            ResourceState state,
            Project project,
            GroupManifest.GroupScopeEnum scope,
            IdentityOrigin origin)
        {
            var membership = await graphService.GetGroupMembersAsync(group.Descriptor);

            if (membership != null && GroupManifest.Membership != null)
            {
                if (GroupManifest.Membership.Groups != null)
                {
                    foreach (var expectedGroup in GroupManifest.Membership.Groups)
                    {
                        var foundGroup = await graphService.GetGroupAsync(
                            expectedGroup.Name, project.Name, 
                            expectedGroup.Scope, expectedGroup.Origin, expectedGroup.AadObjectId);

                        if (foundGroup == null)
                        {
                            state.AddError($"{expectedGroup.Name} doesn't exist");
                        }
                        else
                        {
                            var alreadyMember = membership.Members
                                .FirstOrDefault(m => m.MemberDescriptor.Equals(foundGroup.Descriptor, StringComparison.OrdinalIgnoreCase));

                            if(alreadyMember == null)
                            {
                                state.AddProperty($"Membership", $"{expectedGroup.Name} will be added.", true);
                            }
                        }
                    }
                }

                if (GroupManifest.Membership.Users != null)
                {
                    foreach (var expectedUser in GroupManifest.Membership.Users)
                    {
                        var foundUser = await graphService.GetUserByPrincipalNameAsync(expectedUser.Principal);

                        if (foundUser == null)
                        {
                            state.AddError($"{expectedUser.Principal} doesn't exist");
                        }
                        else
                        {
                            var alreadyMember = membership.Members
                                .FirstOrDefault(m => m.MemberDescriptor.Equals(foundUser.Descriptor, StringComparison.OrdinalIgnoreCase));

                            if (alreadyMember == null)
                            {
                                state.AddProperty($"Membership", $"{foundUser.PrincipalName} will be added.", true);
                            }
                        }
                    }
                }
            }
        }



        protected override Type GetResourceType()
        {
            return typeof(GroupManifest);
        }

        private GroupManifest GroupManifest { get { return (GroupManifest)this.Manifest; } }
        #endregion
    }
}
