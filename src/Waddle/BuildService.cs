using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Waddle.Dtos;
using Waddle.Supports;

namespace Waddle
{
    public class BuildService : RestServiceBase
    {
        public BuildService(string adoUrl, string pat)
            : base(adoUrl, pat)
        {

        }

        public async Task<VstsFolderCollection> ListFoldersAsync(Guid projectId, string path = "")
        {
            var requestPath = $"{projectId}/_apis/build/folders/{path}?api-version=6.0-preview.2";
            var folders = await GetAzureDevOpsDefaultUri()
                .GetRestAsync<VstsFolderCollection>(requestPath, await GetBearerTokenAsync());

            return folders;
        }


        public async Task<VstsFolder> CreateFolderAsync(
                Guid project, string path)
        {
            var response = await GetAzureDevOpsDefaultUri()
                .PutRestAsync(
                $"{project}/_apis/build/folders?path={HttpUtility.UrlEncode(path)}&api-version=6.0-preview.2",
                new
                { 
                    path = path
                },
                await GetBearerTokenAsync());

            return JsonConvert.DeserializeObject<VstsFolder>(response);
        }
    }
}