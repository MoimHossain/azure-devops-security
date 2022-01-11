

using Kdoctl.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.Tasks
{
    public partial class ExportTask
    {
        protected async Task ExportProjectPermissionsAsync(AzDoServices.Dtos.Project project)
        {
            var graphService = Factory.GetGroupService();
            var groups = await graphService.ListGroupsInProjectAsync(project.Id);
            var manifest = BaseSchema.GetEmpty(project.Name, ManifestKind.Permission);
            manifest.Permissions = new List<PermissionSchemaManifest>();

            foreach (var group in groups)
            {
                var memberCollection = await graphService.GetGroupMembersAsync(group.Descriptor);
                if (memberCollection != null && memberCollection.Members != null && memberCollection.Members.Any())
                {
                    var permissionManifest = PermissionSchemaManifest.Create(group.DisplayName);
                    manifest.Permissions.Add(permissionManifest);

                    await PopulateMembershipAsync(graphService, memberCollection, permissionManifest);
                }
            }

            if(manifest.Permissions.Any())
            {
                await this.fs.WriteManifestAsync(project, ManifestKind.Permission, Serialize(manifest));
            }            
        }

        private static async Task PopulateMembershipAsync(
            AzDoServices.GraphService graphService, 
            AzDoServices.Dtos.VstsMembershipCollection memberCollection, 
            PermissionSchemaManifest permissionManifest)
        {
            foreach (var member in memberCollection.Members)
            {
                if (graphService.IsGroupDescriptor(member.MemberDescriptor))
                {
                    var gp = await graphService.GetGroupByDescriptorAsync(member.MemberDescriptor);
                    if (gp != null)
                    {
                        permissionManifest.Membership.Groups.Add(new AadObjectSchema
                        {
                            Name = gp.DisplayName,
                            Id = gp.OriginId
                        });
                    }
                }
                else
                {
                    var usr = await graphService.GetUserByDescriptorAsync(member.MemberDescriptor);
                    if (usr != null)
                    {
                        permissionManifest.Membership.Users.Add(new AadObjectSchema
                        {
                            Name = usr.DisplayName,
                            Id = usr.OriginId
                        });
                    }
                }
            }
        }
    }
}
