using Cielo.Azdo.Abstract;
using Cielo.Azdo.Dtos;
using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /* Get Admin
         use this class in DTO VstsTeamAdminResponseRoot


###
GET  https://vssps.{{host}}/{{organization}}/_apis/graph/descriptors/{{teamID}}
Authorization: Basic {{base64EncodedPat}}
Content-Type: application/json

###
POST https://dev.azure.com/moim/_apis/Contribution/HierarchyQuery?api-version=5.0-preview.1
Authorization: Basic {{base64EncodedPat}}
Content-Type: application/json

{
	"contributionIds": ["ms.vss-admin-web.admin-teams-admin-list-data-provider"],
	"dataProviderContext": {
		"properties": {
			"descriptor": "vssgp.Uy0xLTktMTU1MTM3NDI0NS0yOTg0MTI4OTgzLTMzNTkyMzAwOS0yNTAyNjQwNDUwLTg0Mzk1OTYzLTEtNTkzNDk1Mzc5LTQyMDU3MzA4ODItMjI3Njk0NTg0My00MjUxMzE4NzA5"
		}
	}
}*/
    }
}
