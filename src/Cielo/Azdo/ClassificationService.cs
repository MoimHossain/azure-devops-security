using Cielo.Azdo.Abstract;
using Cielo.Azdo.Dtos;
using Newtonsoft.Json.Linq;
using YamlDotNet.Core.Tokens;

namespace Cielo.Azdo
{
    public class ClassificationService : RestServiceBase
    {
        private readonly SecurityNamespaceService securityNamespaceService;

        public ClassificationService(
            SecurityNamespaceService securityNamespaceService, IHttpClientFactory clientFactory) 
            : base(clientFactory) 
        {
            this.securityNamespaceService = securityNamespaceService;
        }

        public async Task<VstsClassification> GetIterationPathAsync(Guid projectId, string iterationPath)
        {
            var path = GetNormalizedPath(iterationPath);
            var nodes = await GetFlatternedIterationPathsAsync(projectId, 10);
            var clsNode = nodes.FirstOrDefault(node => node.TrimmedPath.Equals(path, StringComparison.OrdinalIgnoreCase));
            return clsNode;
        }

        public async Task<VstsClassification> GetAreaPathAsync(Guid projectId, string areaPath)
        {
            var path = GetNormalizedPath(areaPath);
            var nodes = await GetFlatternedAreaPathsAsync(projectId, 10);
            var clsNode = nodes.FirstOrDefault(node => node.TrimmedPath.Equals(path, StringComparison.OrdinalIgnoreCase));
            return clsNode;
        }

        public string GetSecurityToken(Guid projectId, Guid nodeId)
        {
            return $"vstfs:///Classification/Node/{nodeId}";
        }

        public async Task<Guid> GetAreaPathNamespaceId()
        {
            var ns = await securityNamespaceService.GetNamespaceAsync(SecurityNamespaceService.SecurityNamespaceConstants.Classifications);
            return ns.NamespaceId;
        }

        public async Task<Guid> GetIterationPathNamespaceId()
        {
            var ns = await securityNamespaceService.GetNamespaceAsync(SecurityNamespaceService.SecurityNamespaceConstants.Iteration);
            return ns.NamespaceId;
        }


        public async Task<bool> SetIterationPathPermissionsAsync(Guid projectId, VstsClassification node, List<VstsAcesDictionaryEntry> acls)
        {
            if (node != null && node.ParentNodeIds != null)
            {
                var nsID = await GetIterationPathNamespaceId();
                var tokenCollection = node.ParentNodeIds.Select(pid => GetSecurityToken(projectId, pid)).ToList();
                tokenCollection.Add(GetSecurityToken(projectId, node.Identifier));
                var finalToken = string.Join(':', tokenCollection);

                var payload = new
                {
                    token = finalToken,
                    merge = true,
                    accessControlEntries = acls
                };
                return await CoreApi().PostWithoutResponseBodyAsync($"_apis/AccessControlEntries/{nsID}?api-version=6.0", payload);
            }
            return false;
        }

        public async Task<List<VstsSubjectPermission>> GetIterationPathPermissionsAsync(Guid projectId, string subjectDescriptor, Guid nodeId)
        {
            var token = GetSecurityToken(projectId, nodeId);
            var ns = await GetIterationPathNamespaceId();
            var apiPath = $"_apis/Contribution/HierarchyQuery/project/{projectId}?api-version=5.0-preview.1";
            var permissionResponse = await CoreApi().PostRestAsync<VstsRepoPermissionRoot>(apiPath,
                new
                {
                    contributionIds = new string[] { "ms.vss-admin-web.security-view-permissions-data-provider" },
                    dataProviderContext = new
                    {
                        properties = new
                        {
                            subjectDescriptor = subjectDescriptor,
                            permissionSetId = ns,
                            permissionSetToken = token
                        }
                    }
                });

            if (permissionResponse != null
                && permissionResponse.DataProviders != null
                && permissionResponse.DataProviders.MsVssAdminWebSecurityViewPermissionsDataProvider != null
                && permissionResponse.DataProviders.MsVssAdminWebSecurityViewPermissionsDataProvider.SubjectPermissions != null)
            {
                return permissionResponse.DataProviders.MsVssAdminWebSecurityViewPermissionsDataProvider.SubjectPermissions;
            }
            return new List<VstsSubjectPermission>();
        }





        public async Task<bool> SetAreaPathPermissionsAsync(Guid projectId, VstsClassification node, List<VstsAcesDictionaryEntry> acls)
        {
            if(node != null && node.ParentNodeIds != null )
            {
                var nsID = await GetAreaPathNamespaceId();
                var tokenCollection = node.ParentNodeIds.Select(pid => GetSecurityToken(projectId, pid)).ToList();
                tokenCollection.Add(GetSecurityToken(projectId, node.Identifier));
                var finalToken = string.Join(':', tokenCollection);

                var payload = new
                {
                    token = finalToken,
                    merge = true,
                    accessControlEntries = acls
                };
                return await CoreApi().PostWithoutResponseBodyAsync($"_apis/AccessControlEntries/{nsID}?api-version=6.0", payload);
            }
            return false;
        }

        public async Task<List<VstsSubjectPermission>> GetAreaPathPermissionsAsync(Guid projectId, string subjectDescriptor, Guid nodeId)
        {
            var token = GetSecurityToken(projectId, nodeId);
            var ns = await GetAreaPathNamespaceId();
            var apiPath = $"_apis/Contribution/HierarchyQuery/project/{projectId}?api-version=5.0-preview.1";
            var permissionResponse = await CoreApi().PostRestAsync<VstsRepoPermissionRoot>(apiPath,
                new
                {
                    contributionIds = new string[] { "ms.vss-admin-web.security-view-permissions-data-provider" },
                    dataProviderContext = new
                    {
                        properties = new
                        {
                            subjectDescriptor = subjectDescriptor,
                            permissionSetId = ns,
                            permissionSetToken = token
                        }
                    }
                });

            if (permissionResponse != null
                && permissionResponse.DataProviders != null
                && permissionResponse.DataProviders.MsVssAdminWebSecurityViewPermissionsDataProvider != null
                && permissionResponse.DataProviders.MsVssAdminWebSecurityViewPermissionsDataProvider.SubjectPermissions != null)
            {
                return permissionResponse.DataProviders.MsVssAdminWebSecurityViewPermissionsDataProvider.SubjectPermissions;
            }
            return new List<VstsSubjectPermission>();
        }

        private void FilloutNodes(List<Guid> parentNodeIDs, IEnumerable<VstsClassification> nodes, List<VstsClassification> copyTo)
        {
            foreach (var node in nodes)
            {
                node.ParentNodeIds.AddRange(parentNodeIDs);
                copyTo.Add(node);

                var nextChildsParentId = new List<Guid>();
                nextChildsParentId.AddRange(parentNodeIDs);
                nextChildsParentId.Add(node.Identifier);

                if (node.Children != null && node.Children.Length > 0)
                {
                    FilloutNodes(nextChildsParentId, node.Children, copyTo);
                }
            }
        }

        public async Task<List<VstsClassification>> GetFlatternedIterationPathsAsync(Guid projectId, int depth = 1)
        {
            var flatList = new List<VstsClassification>();
            var rootNode = await GetAllIterationPathsAsync(projectId, depth);
            if (rootNode != null)
            {
                FilloutNodes(new List<Guid>(), new List<VstsClassification> { rootNode }, flatList);
            }
            return flatList;
        }

        public async Task<VstsClassification> GetAllIterationPathsAsync(Guid projectId, int depth = 1)
        {
            var path = $"{projectId}/_apis/wit/classificationnodes/Iterations?$depth={depth}&api-version=6.0";
            var paths = await CoreApi()
                .GetRestAsync<VstsClassification>(path);

            return paths;
        }

        public async Task<List<VstsClassification>> GetFlatternedAreaPathsAsync(Guid projectId, int depth = 1)
        {
            var flatList = new List<VstsClassification>();
            var rootNode = await GetAllAreaPathsAsync(projectId, depth);
            if(rootNode != null)
            {
                FilloutNodes(new List<Guid>(), new List<VstsClassification> { rootNode }, flatList);
            }
            return flatList;
        }

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


        public async Task<VstsClassification> CreateOrUpdateIterationPathAsync(Guid projectId, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            path = GetNormalizedPath(path);

            var segments = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (segments != null && segments.Length > 0)
            {
                var rootNode = await GetAllIterationPathsAsync(projectId, 10);
                var parentId = rootNode.Identifier;
                foreach (var segment in segments.Skip(1))
                {
                    var nodeInfo = await EnsureIterationPathExistsAsync(projectId, rootNode, segment, parentId);
                    parentId = nodeInfo.Node.Id;
                }
            }

            return await this.GetIterationPathAsync(projectId, path);
        }



        public async Task<VstsClassification> CreateOrUpdateAreaPathAsync(Guid projectId, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            path = GetNormalizedPath(path);

            var segments = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (segments != null && segments.Length > 0)
            {
                var rootNode = await GetAllAreaPathsAsync(projectId, 10);
                var parentId = rootNode.Identifier;
                foreach (var segment in segments.Skip(1))
                {
                    var nodeInfo = await EnsureAreaPathExistsAsync(projectId, rootNode, segment, parentId);
                    parentId = nodeInfo.Node.Id;
                }
            }

            return await this.GetAreaPathAsync(projectId, path);
        }

        private VstsClassification CheckIfNodeAlreadyExists(
            VstsClassification parentNode, Guid parentId, string nodeName)
        {
            if (parentNode.Children != null && parentNode.Children.Any())
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
            EnsureIterationPathExistsAsync(
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

            var apiPath = $"{projectId}/_admin/_Iterations/CreateClassificationNode?useApiUrl=true&__v=5";
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

        public static string GetNormalizedPath(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                path = path.TrimStart(new char[] { '/', '\\' }).Replace("\\", "/");
                var parts = path.Split("/", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts != null && parts.Length > 0)
                {
                    return string.Join('/', parts);
                }
            }
            return string.Empty;
        }
    }


}
