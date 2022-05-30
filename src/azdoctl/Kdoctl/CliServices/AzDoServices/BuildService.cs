


using Kdoctl.CliServices.Abstract;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Supports;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Kdoctl.CliServices.AzDoServices
{
    public class BuildService : RestServiceBase
    {
        public BuildService(IHttpClientFactory clientFactory) : base(clientFactory) { }


        public async Task<VstsFolderCollection> ListFoldersAsync(Guid projectId, string path = "")
        {
            var requestPath = $"{projectId}/_apis/build/folders/{path}?api-version=6.0-preview.2";
            var folders = await CoreApi()
                .GetRestAsync<VstsFolderCollection>(requestPath);

            return folders;
        }


        public async Task<VstsFolder> CreateFolderAsync(
                Guid project, string path)
        {
            var response = await CoreApi()
                .PutRestAsync(
                $"{project}/_apis/build/folders?path={HttpUtility.UrlEncode(path)}&api-version=6.0-preview.2",
                new
                { 
                    path = path
                });

            return JsonConvert.DeserializeObject<VstsFolder>(response);
        }

        public async Task<VstsPipelineCollection> ListPipelinesAsync(Guid projectId)
        {   
            var requestPath = $"{projectId}/_apis/pipelines?api-version=6.0-preview.1";
            var pipelines = await CoreApi().GetRestAsync<VstsPipelineCollection>(requestPath);

            return pipelines;
        }

        public async Task<VstsBuildDefinitionCollection> ListBuildDefinitionsAsync(Guid projectId)
        {
            var requestPath = $"{projectId}/_apis/build/definitions?api-version=6.0";
            var defs = await CoreApi().GetRestAsync<VstsBuildDefinitionCollection>(requestPath);

            return defs;
        }        

        public async Task<VstsBuildDefinition> GetBuildDefinitionsAsync(Guid projectId, long definitionId)
        {
            var requestPath = $"{projectId}/_apis/build/definitions/{definitionId}?api-version=6.0";
            var defs = await CoreApi().GetRestAsync<VstsBuildDefinition>(requestPath);

            return defs;
        }
    }
}