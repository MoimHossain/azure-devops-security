using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.AzDoServices
{
    public class RepositoryService
    {
        private GitHttpClient gitClient;

        public RepositoryService(GitHttpClient gitClient)
        {
            this.gitClient = gitClient;
        }

        public async Task<List<GitRepository>> GetRepositoryListAsync()
        {
            var allRepos = await this.gitClient.GetRepositoriesAsync();
            return allRepos;
        }

        public async Task<List<GitRepository>> GetRepositoryListAsync(Guid projectId)
        {
            var allRepos = await this.gitClient.GetRepositoriesAsync(projectId);
            return allRepos;
        }

        public async Task<GitRepository> CreateAsync(Guid id, string name)
        {
            return await this.gitClient.CreateRepositoryAsync(new GitRepositoryCreateOptions
            {
                Name = name,
                ProjectReference = new TeamProjectReference
                {
                    Id = id
                }
            });
        }

        public async Task DeleteRepositoryAsync(Guid projectId, Guid repositoryId)
        {
            await this.gitClient.DeleteRepositoryAsync(projectId, repositoryId);
        }
    }
}
