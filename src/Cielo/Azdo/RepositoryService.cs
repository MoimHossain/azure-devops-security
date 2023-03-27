
using Cielo.Azdo.Abstract;
using Cielo.Azdo.Dtos;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo
{
    public class RepositoryService : RestServiceBase
    {
        private readonly SecurityNamespaceService securityNamespaceService;
        private readonly GitHttpClient gitHttpClient;

        public RepositoryService(
            SecurityNamespaceService securityNamespaceService,
            GitHttpClient gitHttpClient,
            IHttpClientFactory clientFactory) : base(clientFactory)
        {
            this.securityNamespaceService = securityNamespaceService;
            this.gitHttpClient = gitHttpClient;
        }

        public async Task<GitRepository?> GetRepositoryAsync(Guid projectId, string repositoryName)
        {
            var allRepos = await this.gitHttpClient.GetRepositoriesAsync(projectId.ToString());
            return allRepos.FirstOrDefault(repo => repo.Name.Equals(repositoryName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Guid> GetNamespaceId()
        {
            var ns = await securityNamespaceService.GetNamespaceAsync(SecurityNamespaceService.SecurityNamespaceConstants.Git_Repositories);
            return ns.NamespaceId;
        }

        public async Task<List<VstsSubjectPermission>> GetPermissionsAsync(
            Guid projectId, string subjectDescriptor, Guid repositoryId)
        {
            return await GetRepoPermissionsCoreAsync(subjectDescriptor, $"repoV2/{projectId}/{repositoryId}");
        }

        public async Task<List<VstsSubjectPermission>> GetRootPermissionsAsync(Guid projectId, string subjectDescriptor)
        {
            return await GetRepoPermissionsCoreAsync(subjectDescriptor, $"repoV2/{projectId}/");
        }

        private async Task<List<VstsSubjectPermission>> GetRepoPermissionsCoreAsync(string subjectDescriptor, string tokenName)
        {
            var ns = await GetNamespaceId();
            var path = "_apis/Contribution/HierarchyQuery?api-version=5.0-preview.1";
            var permissionResponse = await CoreApi().PostRestAsync<VstsRepoPermissionRoot>(path,
                new
                {
                    contributionIds = new string[] { "ms.vss-admin-web.security-view-permissions-data-provider" },
                    dataProviderContext = new
                    {
                        properties = new
                        {
                            subjectDescriptor = subjectDescriptor,
                            permissionSetId = ns,
                            permissionSetToken = tokenName
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
    }
}
