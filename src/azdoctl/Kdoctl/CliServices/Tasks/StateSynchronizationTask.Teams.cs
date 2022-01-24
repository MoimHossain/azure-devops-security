

using Kdoctl.CliServices.AzDoServices;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Constants;
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
        protected async Task EnsureTeamProvisionedAsync(
            ProjectManifest manifest,
            
            ProjectService projectService,
            Tuple<Kdoctl.CliServices.AzDoServices.Dtos.Project, bool> outcome)
        {
            if(manifest.Teams != null && manifest.Teams.Any())
            {
                var gService = GetGraphService();
                var secService = GetSecurityNamespaceService();
                var aclService = GetAclListService();
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
                        var breakOut = 0;
                        while (eteam == null)
                        {
                            tc = await projectService.GetTeamsAsync();
                            eteam = tc.Value
                                .FirstOrDefault(tc => tc.Name.Equals(teamManifest.Name,
                                StringComparison.OrdinalIgnoreCase));

                            if (++breakOut > 10)
                            {
                                throw new InvalidOperationException($"Team [{teamManifest.Name}] was not retrieved on time.");
                            }
                        }
                        Logger.StatusEndSuccess("Succeed");
                    }

                    if (eteam != null && teamManifest.Membership != null
                        && (teamManifest.Membership.Groups != null && teamManifest.Membership.Groups.Any()))
                    {
                        var teamGroup = await GetGroupByNameAsync( IdentityOrigin.Vsts.ToString(), eteam.Name);
                        if (teamGroup != null)
                        {
                            foreach (var gp in teamManifest.Membership.Groups)
                            {
                                var groupObject = await GetGroupByNameAsync( IdentityOrigin.Aad.ToString(), gp.Name, gp.Id);

                                if (groupObject != null)
                                {
                                    await gService.AddMemberAsync(eteam.ProjectId, teamGroup.Descriptor, groupObject.Descriptor);
                                }
                            }


                            foreach (var user in teamManifest.Membership.Users)
                            {
                                var userInfo = allUsers.Value.FirstOrDefault(u => u.OriginId.Equals(user.Id));
                                if (userInfo != null)
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
        }
    }
}
