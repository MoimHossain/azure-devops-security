

using Kdoctl.CliServices.AzDoServices;
using Kdoctl.CliServices.AzDoServices.Dtos;
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
        protected async Task ProcessPermissionsAsync(
            ProjectManifest manifest,
            ProjectService projectService,
            Tuple<Kdoctl.CliServices.AzDoServices.Dtos.Project, bool> outcome)
        {
            if (manifest.Permissions != null && manifest.Permissions.Any())
            {
                var gService = GetGraphService();
                var allUsers = await gService.ListUsersAsync();
                var projectId = outcome.Item1.Id;

                foreach (var permissionEntry in manifest.Permissions)
                {
                    if (!string.IsNullOrWhiteSpace(permissionEntry.Name) && permissionEntry.Membership != null)
                    {
                        var projectGroups = await gService.ListGroupsInProjectAsync(projectId);
                        if (projectGroups != null)
                        {
                            var targetGroup = projectGroups
                                .FirstOrDefault(pg => pg.DisplayName.Equals(permissionEntry.Name, StringComparison.OrdinalIgnoreCase));
                            if (targetGroup != null)
                            {
                                await ApplyGroupPermissionsAsync( gService, allUsers, projectId, permissionEntry, targetGroup);
                            }
                        }
                    }
                }
            }
        }

        private async Task ApplyGroupPermissionsAsync(            
            GraphService gService, 
            GroupCollection allUsers, 
            Guid projectId, 
            PermissionSchemaManifest permissionEntry, 
            VstsGroup targetGroup)
        {
            using var op = Insights.BeginOperation($"Updating membership of [{targetGroup.PrincipalName}]...", "GroupPermissions");
            // get existing members - so later we can remove the unwanted members (no longer in yaml)
            var outdatedMembership = await gService.GetGroupMembersAsync(targetGroup.Descriptor);
            var survivorDescriptors = new List<string>();

            if (permissionEntry.Membership.Groups != null && permissionEntry.Membership.Groups.Any())
            {
                foreach (var gp in permissionEntry.Membership.Groups)
                {
                    var groupObject = await GetGroupByNameAsync(IdentityOrigin.Aad.ToString(), gp.Name, gp.Id);
                    if (groupObject != null)
                    {
                        await gService.AddMemberAsync(projectId, targetGroup.Descriptor, groupObject.Descriptor);
                        survivorDescriptors.Add(groupObject.Descriptor);
                    }
                }
            }

            if(permissionEntry.Membership.Users != null && permissionEntry.Membership.Users.Any())
            {
                foreach (var user in permissionEntry.Membership.Users)
                {
                    var userInfo = allUsers.Value.FirstOrDefault(u => u.OriginId.Equals(user.Id));
                    if (userInfo != null)
                    {
                        await gService.AddMemberAsync(projectId, targetGroup.Descriptor, userInfo.Descriptor);
                        survivorDescriptors.Add(userInfo.Descriptor);
                    }
                }
            }

            if(outdatedMembership != null && outdatedMembership.Members != null && outdatedMembership.Members.Any())
            {
                foreach(var potentialOutdatedMember in outdatedMembership.Members)
                {
                    var remainValid = survivorDescriptors
                        .Exists(s => s.Contains(potentialOutdatedMember.MemberDescriptor, StringComparison.OrdinalIgnoreCase));
                    if(!remainValid)
                    {
                        await gService.RemoveMembershipAsync(projectId, targetGroup.Descriptor, potentialOutdatedMember.MemberDescriptor);
                    }
                }
            }            
        }

        protected async Task CreateAclsAsync(            
            Type enumType,
            List<PermissionManifest> permissions, Dictionary<string, VstsAcesDictionaryEntry> aclDictioanry)
        {
            foreach (var permission in permissions)
            {
                if (permission.Allowed != null && permission.Allowed.Any())
                {
                    var group = await GetGroupByNameAsync( permission.Origin, permission.Group);

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
    }
}
