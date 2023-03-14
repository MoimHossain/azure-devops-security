

using Kdoctl.CliServices.AzDoServices;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Constants;
using Kdoctl.Schema;
using Kdoctl.Schema.CliServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
                var currentProject = outcome.Item1;
                var gService = GetGraphService();
                var secService = GetSecurityNamespaceService();
                var aclService = GetAclListService();
               // var allUsers = await gService.ListUsersAsync();
                foreach (var teamManifest in manifest.Teams)
                {
                    var tc = await projectService.GetTeamsAsync(currentProject.Id);
                    var eteam = tc.Value
                        .FirstOrDefault(tc => tc.Name.Equals(teamManifest.Name,
                        StringComparison.OrdinalIgnoreCase));

                    if (eteam == null)
                    {
                        ConsoleLogger.NewLineMessage($"Creating team [{teamManifest.Name}]...");
                        using var op = Insights.BeginOperation($"Creating team [{teamManifest.Name}]...", "Team");
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
                            tc = await projectService.GetTeamsAsync(currentProject.Id);
                            eteam = tc.Value
                                .FirstOrDefault(tc => tc.Name.Equals(teamManifest.Name,
                                StringComparison.OrdinalIgnoreCase));

                            if (++breakOut > 10)
                            {
                                op.EndWithFailure($"Team [{teamManifest.Name}] was not retrieved on time.");
                                throw new InvalidOperationException($"Team [{teamManifest.Name}] was not retrieved on time.");
                            }
                        }                        
                    }                                       
                    
                    if (eteam != null &&
                        teamManifest.Admins != null &&
                        teamManifest.Admins.Any())
                    {
                        var token = $"{eteam.ProjectId}\\{eteam.Id}";
                        var securityNamespace = await secService.GetNamespaceAsync(SecurityNamespaceConstants.Identity);
                        var secNamespaceId = securityNamespace.NamespaceId;
                        var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();

                        foreach (var adminUser in teamManifest.Admins)
                        {
                            var loadedUserObject = await gService.GetUserByPrincipalNameAsync(adminUser.Principal);
                            if(loadedUserObject != null)
                            {
                                var securityDescriptor = gService.GetSecurityDescriptorForUser(loadedUserObject);
                                aclDictioanry.Add(securityDescriptor, new VstsAcesDictionaryEntry
                                {
                                    Allow = 31,
                                    Deny = 0,
                                    Descriptor = securityDescriptor
                                });
                            }

                            // the following code works but I am trying the above approach
                            //var matches = await gService.GetLegacyIdentitiesByNameAsync(adminUser.Name);
                            //if (matches != null && matches.Count > 0)
                            //{
                            //    var adminUserInfo = matches.Value.First();
                            //    aclDictioanry.Add(adminUserInfo.Descriptor, new VstsAcesDictionaryEntry
                            //    {
                            //        Allow = 31,
                            //        Deny = 0,
                            //        Descriptor = adminUserInfo.Descriptor
                            //    });
                            //}
                        }
                        var result = await aclService.SetAclsAsync(secNamespaceId, token, aclDictioanry, false);
                        ConsoleLogger.NewLineMessage($"Added admins to {eteam.Name} was {(result ? "Successfull" : "Failed")}");
                    }
                    
                    if (eteam != null && teamManifest.Membership != null)
                    {
                        var teamGroupIdentity = await gService.GetIdentityObjectAsync(eteam.Id);

                        if (teamGroupIdentity != null)
                        {
                            if (teamManifest.Membership.Groups != null)
                            {
                                foreach (var gp in teamManifest.Membership.Groups)
                                {
                                    var groupObject = await GetOrMaterializeGroupAsync(gp.Name, gp.Id);

                                    if (groupObject != null)
                                    {
                                        var operationResult = await gService.AddMemberAsync(eteam.ProjectId, teamGroupIdentity.SubjectDescriptor, groupObject.Descriptor);
                                        ConsoleLogger.NewLineMessage($"Adding group {groupObject.DisplayName} was {(operationResult ? "successful" : "failed")}");
                                    }
                                    else
                                    {
                                        ConsoleLogger.NewLineMessage($"Failed to find group in identity: {gp.Name}");
                                    }
                                }
                            }

                            if (teamManifest.Membership.Users != null)
                            {
                                foreach (var user in teamManifest.Membership.Users)
                                {
                                    var loadedUserObject = await gService.GetUserByPrincipalNameAsync(user.Principal);
                                    if (loadedUserObject != null)
                                    {
                                        ConsoleLogger.NewLineMessage($"Adding member {user.Principal}");
                                        var operationResult = await gService.AddMemberAsync(
                                            eteam.ProjectId,
                                            teamGroupIdentity.SubjectDescriptor,
                                            loadedUserObject.Descriptor);

                                        ConsoleLogger.NewLineMessage($"Adding member {user.Principal} was {(operationResult ? "successful" : "failed")}");
                                    }
                                    else
                                    {
                                        ConsoleLogger.NewLineMessage($"Failed to find member in identity: {user.Name}");
                                    }
                                }
                            }
                        }
                    }



                }
            }
        }
    }
}
