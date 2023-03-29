

using Kdoctl.CliServices.Abstract;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace Kdoctl.CliServices.AzDoServices
{
    public class ClassificationService : RestServiceBase
    {
        public ClassificationService(IHttpClientFactory clientFactory) : base(clientFactory) { }

        public async Task<VstsClassification> GetAllAreaPathsAsync(Guid projectId, int depth = 1)
        {
            var path = $"{projectId}/_apis/wit/classificationnodes/Areas?$depth={depth}&api-version=6.0";
            var paths = await CoreApi()
                .GetRestAsync<VstsClassification>(path);

            return paths;
        }

        public async Task<VstsClassification> GetAllIterationsPathsAsync(Guid projectId, int depth = 1)
        {
            var path = $"{projectId}/_apis/wit/classificationnodes/Iterations?$depth={depth}&api-version=6.0";
            var paths = await CoreApi()
                .GetRestAsync<VstsClassification>(path);

            return paths;
        }

        public async Task CreateOrUpdateAreaPathAsync(Guid projectId, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            var segments = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (segments != null && segments.Length > 0)
            {
                var rootNode = await GetAllAreaPathsAsync(projectId, 10);
                var parentId = rootNode.Identifier;
                foreach (var segment in segments)
                {
                    var nodeInfo = await EnsureAreaPathExistsAsync(projectId, rootNode, segment, parentId);
                    parentId = nodeInfo.Node.Id;
                }
            }
            await Task.CompletedTask;
        }

        private VstsClassification CheckIfNodeAlreadyExists(
            VstsClassification parentNode, Guid parentId, string nodeName)
        {
            if(parentNode.Children != null && parentNode.Children.Any())
            {
                if (parentNode.Identifier.Equals(parentId))
                {
                    return parentNode.Children
                            .FirstOrDefault(ch => ch.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    foreach (var child in parentNode.Children)
                    {
                        var foundNode = CheckIfNodeAlreadyExists(child, parentId, nodeName);
                        if (foundNode != null)
                        {
                            return foundNode;
                        }
                    }
                }
            }
            return default;
        }

        private async Task<WorkItemClassificationCreateOrUpdateResponse> 
            EnsureAreaPathExistsAsync(
            Guid projectId, 
            VstsClassification parentNode, string segment, Guid parentId)
        {
            var foundNode = CheckIfNodeAlreadyExists(parentNode, parentId, segment);

            if (foundNode != null)
            {
                return new WorkItemClassificationCreateOrUpdateResponse
                {
                    Node = new WorkItemClassificationNodeInfo
                    {
                        Id = foundNode.Identifier,
                        ParentId = parentId,
                        Text = segment
                    }
                };
            }

            var apiPath = $"{projectId}/_admin/_Areas/CreateClassificationNode?useApiUrl=true&__v=5";
            var payload = new WorkItemClassificationNode
            {
                NodeName = segment,
                NodeId = Guid.Empty,
                ParentId = parentId
            };
            var response = await CoreApi()
                .PostRestAsync<WorkItemClassificationCreateOrUpdateResponse>(
                    apiPath, payload.GetCreateOrUpdatePayload());
            return response;
        }
    }
}
