using Cielo.Azdo.Abstract;
using Cielo.Azdo.Dtos;
using Microsoft.TeamFoundation.Core.WebApi;
using Newtonsoft.Json;

namespace Cielo.Azdo
{
    public class TeamService : RestServiceBase
    {
        private readonly TeamHttpClient teamClient;

        public TeamService(TeamHttpClient teamClient, IHttpClientFactory clientFactory) : base(clientFactory)
        {
            this.teamClient = teamClient;
        }

        public async Task<VstsTeam> CreateTeamAsync(WebApiTeam team, Guid projectId)
        {
            var teamCore = await teamClient.CreateTeamAsync(team, projectId.ToString());
            return await GetTeamByNameAsync(projectId, $"{teamCore.Id}");
        }
        
        public async Task<VstsTeam> GetTeamByNameAsync(Guid projectId, string teamName)
        {
            var paramTeamName = RestUtils.UriEncode(teamName);
            var path = $"_apis/projects/{projectId}/teams/{paramTeamName}?api-version=7.0";
            var team = await CoreApi().GetRestAsync<VstsTeam>(path);
            return team;
        }

        public async Task<VstsTeamDescriptor> GetTeamDescriptorAsync(Guid teamId)
        {
            var path = $"_apis/graph/descriptors/{teamId}";
            var teamDescriptor = await VsspsApi().GetRestAsync<VstsTeamDescriptor>(path);
            return teamDescriptor;
        }

        public async Task<List<MsVssTeamAdmin>> GetTeamAdminsAsync(Guid projectId, Guid teamId)
        {
            var teamDescriptor = await this.GetTeamDescriptorAsync(teamId);
            if (teamDescriptor != null && !string.IsNullOrWhiteSpace(teamDescriptor.ScopeDescriptor))
            {
                var adminPath = $"_apis/Contribution/HierarchyQuery?api-version=5.0-preview.1";
                var adminResponse = await CoreApi().PostRestAsync<VstsTeamAdminResponseRoot>(adminPath,
                    new
                    {
                        contributionIds = new string[] { "ms.vss-admin-web.admin-teams-admin-list-data-provider" },
                        dataProviderContext = new
                        {
                            properties = new
                            {
                                descriptor = teamDescriptor.ScopeDescriptor
                            }
                        }
                    });

                if(adminResponse != null && adminResponse.DataProviders != null &&
                    adminResponse.DataProviders.Admins != null )
                {
                    return adminResponse.DataProviders.Admins;
                }
            }
            return new List<MsVssTeamAdmin>();
        }

        public async Task<VstsTeamConfig> GetTeamsAreaPathConfigAsync(Guid projectId, Guid teamId)
        {
            var reqPath = $"{projectId}/{teamId}/_apis/work/teamsettings/teamfieldvalues";
            var response = await CoreApi().GetRestAsync<VstsTeamConfig>(reqPath);

            return response;
        }

        public async Task<bool> UpdateConfigAsync(Guid projectId, Guid teamId, object saveSata)
        {
            var requestURI = $"/{projectId}/{teamId}/_admin/_Areas/UpdateAreasData?useApiUrl=true&teamId={teamId}&__v=5";
            var saveDataJSON = JsonConvert.SerializeObject(saveSata);

            var result = await CoreApi().PostWithoutResponseBodyAsync(requestURI, new { saveData = saveDataJSON });
            return result;
        }
    }
}
