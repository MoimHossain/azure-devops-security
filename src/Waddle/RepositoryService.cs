using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Waddle
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
    }
}
