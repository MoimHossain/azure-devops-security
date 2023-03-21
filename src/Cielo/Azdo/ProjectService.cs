﻿using Cielo.Azdo.Abstract;
using Cielo.Azdo.Dtos;
using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo
{
    public class ProjectService : RestServiceBase
    {
        private readonly TeamHttpClient teamClient;

        public ProjectService(TeamHttpClient teamClient, IHttpClientFactory clientFactory) : base(clientFactory)
        {
            this.teamClient = teamClient;
        }

        public async Task<WebApiTeam> CreateTeamAsync(WebApiTeam team, Guid projectId)
        {
            return await teamClient.CreateTeamAsync(team, projectId.ToString());
        }

        public async Task<VstsTeamCollection> GetTeamsAsync(Guid projectId)
        {
            var path = $"_apis/projects/{projectId}/teams?api-version=7.0";
            var teams = await CoreApi()
                .GetRestAsync<VstsTeamCollection>(path);

            return teams;
        }

        public async Task<string> UpdateRetentionAsync(Guid projectId, ProjectRetentionSetting settings)
        {
            var response = await CoreApi()
               .PatchRestAsync(
               $"{projectId}/_apis/build/retention?api-version=6.0-preview.1",
               settings);

            return response;
        }

        public async Task<Project> GetProjectByIdOrNameAsync(string projectIdOrName)
        {
            var projectParameter = (Guid.TryParse(projectIdOrName, out var projectId))
                ? projectIdOrName : RestUtils.UriEncode(projectIdOrName);

            var path = $"_apis/projects/{projectParameter}?api-version=7.0";
            var project = await CoreApi().GetRestAsync<Project>(path);

            return project;
        }

        public async Task<ProcessTemplateCollection> ListProcessAsync()
        {
            var path = "_apis/process/processes?api-version=6.1-preview.1";
            var processes = await CoreApi()
                .GetRestAsync<ProcessTemplateCollection>(path);
            return processes;
        }

        public async Task<string> CreateProjectAsync(
            string name, ProcessTemplate tempalte,
            string sourceControlType, string description)
        {
            var response = await CoreApi()
                .PostRestAsync(
                $"_apis/projects?api-version=6.1-preview.4",
                new
                {
                    Name = name,
                    Description = description,
                    capabilities = new
                    {
                        versioncontrol = new
                        {
                            sourceControlType
                        },
                        processTemplate = new
                        {
                            templateTypeId = tempalte.Id
                        }
                    }
                });
            return response;
        }
    }
}