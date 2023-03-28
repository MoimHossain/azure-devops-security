using Cielo.Azdo.Abstract;
using Cielo.Azdo.Dtos;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo
{
    public class ReleaseService : RestServiceBase
    {
        private readonly SecurityNamespaceService securityNamespaceService;
        private readonly GitHttpClient gitHttpClient;

        public ReleaseService(
            SecurityNamespaceService securityNamespaceService,
            GitHttpClient gitHttpClient,
            IHttpClientFactory clientFactory) : base(clientFactory)
        {
            this.securityNamespaceService = securityNamespaceService;
            this.gitHttpClient = gitHttpClient;
        }

        public string GetSecurityTokenForPath(Guid projectId, string path)
        {
            var fpath = path.TrimStart("\\/".ToCharArray()).Replace("\\", "/");
            var token = $"{projectId}/{fpath}";
            return token;
        }

        public async Task<Guid> GetNamespaceId()
        {
            var ns = await securityNamespaceService.GetNamespaceAsync(SecurityNamespaceService.SecurityNamespaceConstants.ReleaseManagement);
            return ns.NamespaceId;
        }

        public async Task<List<VstsSubjectPermission>> GetFolderPermissionsAsync(Guid projectId, string subjectDescriptor, string path)
        {
            var token = GetSecurityTokenForPath(projectId, path);
            var ns = await GetNamespaceId();
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

        public async Task<VstsFolder> GetPipelineFolderAsync(Guid projectId, string path = "")
        {
            var requestPath = $"{projectId}/_apis/release/folders/{path}?api-version=6.0-preview.2";
            var folders = await VsrmApi()
                .GetRestAsync<VstsFolderCollection>(requestPath);
            if (folders != null && folders.Value != null)
            {
                var folder = folders.Value.FirstOrDefault(f => f.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
                return folder;
            }
            return null;
        }

        public async Task<VstsFolder> CreateFolderAsync(Guid project, string path)
        {
            var response = await VsrmApi().PostRestAsync(
                $"{project}/_apis/Release/folders{RestUtils.UriEncode(path)}?api-version=6.0-preview.2",
                new
                {
                    path = path
                });

            return JsonConvert.DeserializeObject<VstsFolder>(response);
        }
    }
}
