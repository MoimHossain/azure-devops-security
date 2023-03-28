

using Cielo.Azdo;
using Cielo.Azdo.Dtos;
using Cielo.Manifests;
using Cielo.ResourceManagers.Abstract;
using Cielo.ResourceManagers.ResourceStates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.TeamFoundation.Build.WebApi;
using static Cielo.Azdo.SecurityNamespaceService;
using static Cielo.Manifests.GroupManifest;

namespace Cielo.ResourceManagers
{
    public class TeamResourceManager : ResourceManagerBase
    {
        private readonly TeamService teamService;
        private readonly SecurityNamespaceService sercurityNamespaceService;
        private readonly GraphService graphService;
        private AclService aclService;

        public TeamResourceManager(IServiceProvider serviceProvider, string rawManifest)
            : base(serviceProvider, rawManifest)
        {
            this.teamService = serviceProvider.GetRequiredService<TeamService>();
            this.sercurityNamespaceService = serviceProvider.GetRequiredService<SecurityNamespaceService>();
            this.graphService = serviceProvider.GetRequiredService<GraphService>();
            this.aclService = serviceProvider.GetRequiredService<AclService>();
        }

        protected async override Task<ResourceState> CreateAsync()
        {
            var project = Context.CurrentProject;
            var teamName = this.TeamManifest.Metadata.Name;
            var description = this.TeamManifest.Metadata.Description;
            if (project == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is missing for resource {TeamManifest.Kind}:{teamName}");
                return errorState;
            }

            var state = new ResourceState();
            if (string.IsNullOrEmpty(teamName) || this.TeamManifest == null)
            {
                state.AddError("Team name is empty");
                return state;
            }

            var team = await teamService.CreateTeamAsync(
                new Microsoft.TeamFoundation.Core.WebApi.WebApiTeam
                {
                    Name = teamName,
                    Description = TeamManifest.Metadata.Description,
                    ProjectId = project.Id,
                    ProjectName = project.Name
                }, project.Id);

            if (team != null)
            {
                state.AddProperty("IdentityUrl", team.IdentityUrl);

                await UpdateAdminsAsync(team, state, project);
                await UpdateMembersAsync(team, state, project);
            }
            return state;
        }
        protected async override Task<ResourceState> GetAsync()
        {
            var project = Context.CurrentProject;
            var teamName = TeamManifest.Metadata.Name;
            if (project == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is missing for resource {TeamManifest.Kind}:{teamName}");
                return errorState;
            }

            var state = new ResourceState();
            if (string.IsNullOrEmpty(teamName) || TeamManifest == null)
            {
                state.AddError("Team name is empty");
                return state;
            }

            var team = await teamService.GetTeamByNameAsync(project.Id, teamName);
            state.AddProperty("Name", teamName);
            state.AddProperty("Description", TeamManifest.Metadata.Description);

            if (team == null)
            {
                state.Exists = false;
            }
            else
            {
                state.Exists = true;
                if (TeamManifest.Admins != null)
                {
                    await DiscoverAdminsAsync(team, state, project, TeamManifest.Admins);
                }

                await DiscoverMembershipAsync(team, state, project);
                await DiscoverConfigAsync(team, state, project);
            }
            return state;
        }
        protected async override Task<ResourceState?> UpdateAsync()
        {
            var project = Context.CurrentProject;
            var teamName = TeamManifest.Metadata.Name;
            if (project == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is missing for resource {TeamManifest.Kind}:{teamName}");
                return errorState;
            }

            var state = new ResourceState();
            if (string.IsNullOrEmpty(teamName) || TeamManifest == null)
            {
                state.AddError("Team name is empty");
                return state;
            }

            var team = await teamService.GetTeamByNameAsync(project.Id, teamName);

            if (team != null)
            {
                await UpdateAdminsAsync(team, state, project);
                await UpdateMembersAsync(team, state, project);
            }

            return state;
        }


        private async Task UpdateMembersAsync(VstsTeam team, ResourceState state, Project project)
        {
            var expectedMemberGroups = new List<VstsGroup>();
            var expectedMemberUsers = new List<VstsUserInfo>();

            if (TeamManifest.Membership != null)
            {
                if (TeamManifest.Membership.Groups != null)
                {
                    foreach (var gpManifest in TeamManifest.Membership.Groups)
                    {
                        var group = await graphService.GetGroupAsync(gpManifest.Name, project.Name, gpManifest.Scope, gpManifest.Origin, gpManifest.AadObjectId);
                        if (group != null) { expectedMemberGroups.Add(group); }
                        else { state.AddError($"No descriptor found for {gpManifest.Name}"); }
                    }
                }

                if (TeamManifest.Membership.Users != null)
                {
                    foreach (var userManifest in TeamManifest.Membership.Users)
                    {
                        var user = await graphService.GetUserByPrincipalNameAsync(userManifest.Principal);
                        if (user != null) { expectedMemberUsers.Add(user); }
                        else { state.AddError($"No descriptor found for {userManifest.Principal}"); }
                    }
                }
            }

            var teamDescriptor = await this.teamService.GetTeamDescriptorAsync(team.Id);
            if (teamDescriptor == null)
            {
                state.AddError($"Failed to find {team.Name}'s security descriptor.");
                return;
            }
            var currentMembers = await graphService.GetGroupMembersAsync(teamDescriptor.ScopeDescriptor);
            if (currentMembers == null || currentMembers.Members == null)
            {
                state.AddError($"Failed to read current membership of {team.Name}.");
                return;
            }

            foreach (var expectedGroup in expectedMemberGroups)
            {
                if (!currentMembers.Members.Any(cm => cm.MemberDescriptor.Equals(expectedGroup.Descriptor)))
                {
                    var result = await graphService.AddMemberAsync(project.Id, teamDescriptor.ScopeDescriptor, expectedGroup.Descriptor);
                    if (result)
                    {
                        state.AddProperty("Membership", $"{expectedGroup.PrincipalName} added as member.", true);
                    }
                    else
                    {
                        state.AddError($"Failed to add {expectedGroup.PrincipalName} as member.");
                    }
                }
            }
            foreach (var expectedUser in expectedMemberUsers)
            {
                if (!currentMembers.Members.Any(cm => cm.MemberDescriptor.Equals(expectedUser.Descriptor)))
                {
                    var result = await graphService.AddMemberAsync(project.Id, teamDescriptor.ScopeDescriptor, expectedUser.Descriptor);
                    if (result)
                    {
                        state.AddProperty("Membership", $"{expectedUser.PrincipalName} added as member.", true);
                    }
                    else
                    {
                        state.AddError($"Failed to add {expectedUser.PrincipalName} as member.");
                    }
                }
            }
        }


        private async Task DiscoverAdminsAsync(
            VstsTeam team, ResourceState state,
            Project project, List<UserReference> expectedAdmins)
        {
            var currentAdmins = await teamService.GetTeamAdminsAsync(project.Id, team.Id);

            if (currentAdmins != null)
            {
                foreach (var expectedAdmin in expectedAdmins)
                {
                    var exists = currentAdmins.Exists(ca => ca.UniqueName.Equals(expectedAdmin.Principal, StringComparison.OrdinalIgnoreCase));
                    if (!exists)
                    {
                        state.AddProperty($"{team.Name} Admins", $"{expectedAdmin.Principal} will be added as Admin", true);
                    }
                }
            }
        }

        private async Task UpdateAdminsAsync(VstsTeam team, ResourceState state, Project project)
        {
            var expectedAdmins = TeamManifest.Admins;
            if (expectedAdmins != null && expectedAdmins.Count > 0)
            {
                var token = $"{project.Id}\\{team.Id}";
                var securityNamespace = await sercurityNamespaceService.GetNamespaceAsync(SecurityNamespaceConstants.Identity);
                var secNamespaceId = securityNamespace.NamespaceId;
                var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();

                var currentAdmins = await teamService.GetTeamAdminsAsync(project.Id, team.Id);

                if (currentAdmins != null)
                {
                    foreach (var expectedAdmin in expectedAdmins)
                    {
                        var loadedUserObject = await graphService.GetUserByPrincipalNameAsync(expectedAdmin.Principal);
                        if (loadedUserObject != null)
                        {
                            var securityDescriptor = graphService.GetSecurityDescriptorForUser(loadedUserObject);
                            aclDictioanry.Add(securityDescriptor, new VstsAcesDictionaryEntry
                            {
                                Allow = 31,
                                Deny = 0,
                                Descriptor = securityDescriptor
                            });
                        }
                        else
                        {
                            state.AddError($"Failed to find user {expectedAdmin.Principal}");
                        }
                    }

                    var result = await aclService.SetAclsAsync(secNamespaceId, token, aclDictioanry, false);
                    if (!result)
                    {
                        state.AddError($"Couldn't update Tean Admins");
                    }
                }
            }
        }



        private async Task DiscoverMembershipAsync(VstsTeam team, ResourceState state, Project project)
        {
            var expectedMemberGroups = new List<VstsGroup>();
            var expectedMemberUsers = new List<VstsUserInfo>();

            if (TeamManifest.Membership != null)
            {
                if (TeamManifest.Membership.Groups != null)
                {
                    foreach (var gpManifest in TeamManifest.Membership.Groups)
                    {
                        var group = await graphService.GetGroupAsync(gpManifest.Name, project.Name, gpManifest.Scope, gpManifest.Origin, gpManifest.AadObjectId);
                        if (group != null) { expectedMemberGroups.Add(group); }
                        else { state.AddError($"No descriptor found for {gpManifest.Name}"); }
                    }
                }

                if (TeamManifest.Membership.Users != null)
                {
                    foreach (var userManifest in TeamManifest.Membership.Users)
                    {
                        var user = await graphService.GetUserByPrincipalNameAsync(userManifest.Principal);
                        if (user != null) { expectedMemberUsers.Add(user); }
                        else { state.AddError($"No descriptor found for {userManifest.Principal}"); }
                    }
                }
            }

            var teamDescriptor = await this.teamService.GetTeamDescriptorAsync(team.Id);
            if (teamDescriptor == null)
            {
                state.AddError($"Failed to find {team.Name}'s security descriptor.");
                return;
            }
            var currentMembers = await graphService.GetGroupMembersAsync(teamDescriptor.ScopeDescriptor);
            if (currentMembers == null || currentMembers.Members == null)
            {
                state.AddError($"Failed to read current membership of {team.Name}.");
                return;
            }
            
            foreach(var expectedGroup in expectedMemberGroups)
            {
                if(!currentMembers.Members.Any(cm => cm.MemberDescriptor.Equals(expectedGroup.Descriptor)))
                {
                    state.AddProperty("Member", $"{expectedGroup.PrincipalName} will be added.");
                }                
            }
            foreach (var expectedUser in expectedMemberUsers)
            {
                if (!currentMembers.Members.Any(cm => cm.MemberDescriptor.Equals(expectedUser.Descriptor)))
                {
                    state.AddProperty("Member", $"{expectedUser.PrincipalName} will be added.");
                }
            }
        }

        private async Task DiscoverConfigAsync(VstsTeam team, ResourceState state, Project project)
        {
            var expectedConfig = TeamManifest.Config;
            if(expectedConfig != null)
            {
                var configPropertyBag = new List<(string, object, bool)>();
                state.AddProperty($"Config", configPropertyBag);

                var currentConfig = await this.teamService.GetTeamsAreaPathConfigAsync(project.Id, team.Id);
                if (currentConfig != null)
                {
                    if(!string.IsNullOrWhiteSpace(expectedConfig.DefaultPath))
                    {
                        if (expectedConfig.DefaultPath.Equals(currentConfig.TrimmedDefaultPath, StringComparison.OrdinalIgnoreCase))
                        {
                            configPropertyBag.Add(("Default Area Path", $"{currentConfig.TrimmedDefaultPath} exists.", false));
                        }
                        else 
                        {
                            configPropertyBag.Add(("Default Area Path", $"PATH=({currentConfig.DefaultPath}) will change.", false));
                        }
                    }

                    if(expectedConfig.AreaPaths != null)
                    {
                        foreach(var expectedPath in expectedConfig.AreaPaths)
                        {
                            var foundPath = currentConfig.AreaPaths
                                .FirstOrDefault(ap => ap.TrimmedPath.Equals(expectedPath.Path, StringComparison.OrdinalIgnoreCase)
                                && ap.IncludeChildren == expectedPath.IncludeSubAreas);
                            if (foundPath == null)
                            {
                                configPropertyBag.Add(("Path", $"{expectedPath.Path} (will changes)", false));
                                configPropertyBag.Add(("SubArea included", $"{expectedPath.IncludeSubAreas} (will changes)", false));
                            }
                            else
                            {
                                configPropertyBag.Add(("Path", $"{foundPath.TrimmedPath} (no changes)", false));
                                configPropertyBag.Add(("SubArea included", $"{foundPath.IncludeChildren} (no changes)", false));
                            }
                        }
                    }
                }
            }
        }


        protected override Type GetResourceType()
        {
            return typeof(TeamManifest);
        }

        private TeamManifest TeamManifest { get { return (TeamManifest)this.Manifest; } }
    }
}
