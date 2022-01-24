using Kdoctl.CliServices.Abstract;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Supports;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace Kdoctl.CliServices.AzDoServices
{
    public class ReleaseService : RestServiceBase
    {
        public ReleaseService(IHttpClientFactory clientFactory) : base(clientFactory) { }    

        public async Task<VstsFolderCollection> ListFoldersAsync(Guid projectId, string path = "")
        {
            var requestPath = $"{projectId}/_apis/release/folders/{path}?api-version=6.0-preview.2";
            var folders = await VsrmApi()
                .GetRestAsync<VstsFolderCollection>(requestPath);

            return folders;
        }


        public async Task<VstsFolder> CreateFolderAsync(
                Guid project, string path)
        {
            var response = await VsrmApi().PostRestAsync(             
                $"{project}/_apis/Release/folders{HttpUtility.UrlEncode(path)}?api-version=6.0-preview.2",
                new
                {
                    path = path
                });

            return JsonConvert.DeserializeObject<VstsFolder>(response);
        }
    }
}