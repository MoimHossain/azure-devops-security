using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Waddle
{
    public class AdoConnectionFactory
    {
        private readonly Uri orgUrl;
        private readonly string pat;
        private readonly VssConnection connection;


        public AdoConnectionFactory(Uri orgUrl, string pat)
        {
            this.connection = new VssConnection(orgUrl, new VssBasicCredential(string.Empty, pat));
            this.orgUrl = orgUrl;
            this.pat = pat;
        }

        public AdoConnectionFactory(string orgUri, string pat) : this(new Uri(orgUri), pat)
        {

        }

        public PipelineEnvironmentService GetPipelineEnvironmentService()
        {
            return new PipelineEnvironmentService(this.orgUrl.ToString(), this.pat);
        }

        public ServiceEndpointService GetServiceEndpointService()
        {
            return new ServiceEndpointService(this.orgUrl.ToString(), this.pat);
        }

        public GraphService GetGrouoService()
        {
            return new GraphService(this.orgUrl.ToString(), this.pat);
        }

        public RepositoryService GetRepositoryService()
        {
            var gitClient = connection.GetClient<GitHttpClient>();

            return new RepositoryService(gitClient);
        }

        public SecurityNamespaceService GetSecurityNamespaceService()
        {
            return new SecurityNamespaceService(this.orgUrl.ToString(), this.pat);
        }

        public AclListService GetAclListService()
        {
            return new AclListService(this.orgUrl.ToString(), this.pat);
        }

        public ClassificationService GetClassificationService()
        {
            return new ClassificationService(this.orgUrl.ToString(), this.pat);
        }

        public ProjectService GetProjectService()
        {
            return new ProjectService(this.orgUrl.ToString(), this.pat);
        }

        public ReleaseService GetReleaseService()
        {
            return new ReleaseService(this.orgUrl.ToString(), this.pat);
        }

        public BuildService GetBuildService()
        {
            return new BuildService(this.orgUrl.ToString(), this.pat);
        }
    }
}
